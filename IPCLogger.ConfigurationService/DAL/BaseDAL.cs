﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Runtime.CompilerServices;

namespace IPCLogger.ConfigurationService.DAL
{
    static class SQLiteHost
    {
        private static readonly string _baseFileName = "local.sqlite";

        public static bool ItWasNewDatabase { get; }
        public static HashSet<Type> WhoHasCreatedDbStructureYet { get; }
        public static SQLiteConnection Connection { get; }

        static SQLiteHost()
        {
            WhoHasCreatedDbStructureYet = new HashSet<Type>();
            ItWasNewDatabase = !File.Exists(_baseFileName);

            //if (!ItWasNewDatabase)
            //{
            //    File.Delete(_baseFileName);
            //    ItWasNewDatabase = true;
            //}

            Connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;New={1};Compress=True;",
                _baseFileName, ItWasNewDatabase));
            Connection.Open();
        }
    }

    internal abstract class BaseDAL<TParentType>
        where TParentType : class, new()
    {
        protected SQLiteConnection Connection => SQLiteHost.Connection;

        public static TParentType Instance { get; } = new TParentType();

        protected BaseDAL()
        {
            CheckDBStructure();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void CheckDBStructure()
        {
            Type thisType = GetType();
            if (SQLiteHost.ItWasNewDatabase && !SQLiteHost.WhoHasCreatedDbStructureYet.Contains(thisType))
            {
                CreateDbStructure();
                SQLiteHost.WhoHasCreatedDbStructureYet.Add(thisType);
            }
        }

        protected abstract void CreateDbStructure();
    }
}