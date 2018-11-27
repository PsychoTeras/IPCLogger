(function() {
    var Size = window.Size = function(bytes, kbytes, mbytes, gbytes) {

        var bytesTotal = 0;
        var bytesPerKb = 1024;
        var bytesPerMb = Math.pow(bytesPerKb, 2);
        var bytesPerGb = Math.pow(bytesPerMb, 2);

        var isNumeric = function(input) {
            return !isNaN(parseFloat(input)) && isFinite(input);
        };

        if (bytes && !isNumeric(bytes)) {
            var obj = bytes;
            bytes = obj.bytes;
            kbytes = obj.kbytes;
            mbytes = obj.mbytes;
            gbytes = obj.gbytes;
        }

        // Ctor
        if (isNumeric(bytes)) {
            bytesTotal += bytes;
        }
        if (isNumeric(kbytes)) {
            bytesTotal += kbytes * bytesPerKb;
        }
        if (isNumeric(mbytes)) {
            bytesTotal += mbytes * bytesPerMb;
        }
        if (isNumeric(gbytes)) {
            bytesTotal += gbytes * bytesPerGb;
        }

        // Addition
        this.addBytes = function(bytes) {
            if (!isNumeric(bytes)) {
                return;
            }
            bytesTotal += bytes;
        };
        this.addKBytes = function(kbytes) {
            if (!isNumeric(kbytes)) {
                return;
            }
            bytesTotal += (kbytes * bytesPerKb);
        };
        this.addMBytes = function(mbytes) {
            if (!isNumeric(mbytes)) {
                return;
            }
            bytesTotal += (mbytes * bytesPerMb);
        };
        this.addGBytes = function(gbytes) {
            if (!isNumeric(gbytes)) {
                return;
            }
            bytesTotal += (gbytes * bytesPerGb);
        };

        // Subtraction
        this.subtractBytes = function(bytes) {
            if (!isNumeric(bytes)) {
                return;
            }
            bytesTotal -= bytes;
        };
        this.subtractKBytes = function(kbytes) {
            if (!isNumeric(kbytes)) {
                return;
            }
            bytesTotal -= kbytes * bytesPerKb;
        };
        this.subtractMBytes = function(mbytes) {
            if (!isNumeric(mbytes)) {
                return;
            }
            bytesTotal -= mbytes * bytesPerMb;
        };
        this.subtractGBytes = function(gbytes) {
            if (!isNumeric(gbytes)) {
                return;
            }
            bytesTotal -= gbytes * bytesPerGb;
        };

        // Interact with other instance
        this.isSize = true;
        this.add = function(otherSize) {
            if (!otherSize.isSize) {
                return;
            }
            bytesTotal += otherSize.totalBytes();
        };
        this.subtract = function(otherSize) {
            if (!otherSize.isSize) {
                return;
            }
            bytesTotal -= otherSize.totalBytes();
        };
        this.equals = function(otherSize) {
            if (!otherSize.isSize) {
                return false;
            }
            return bytesTotal === otherSize.totalBytes();
        };

        // Getters
        this.totalBytes = function(roundDown) {
            var result = bytesTotal;
            if (roundDown === true) {
                result = Math.floor(result);
            }
            return result;
        };
        this.totalKBytes = function(roundDown) {
            var result = bytesTotal / bytesPerKb;
            if (roundDown === true) {
                result = Math.floor(result);
            }
            return result;
        };
        this.totalMBytes = function(roundDown) {
            var result = bytesTotal / bytesPerMb;
            if (roundDown === true) {
                result = Math.floor(result);
            }
            return result;
        };
        this.totalGBytes = function(roundDown) {
            var result = bytesTotal / bytesPerGb;
            if (roundDown === true) {
                result = Math.floor(result);
            }
            return result;
        };

        // Return a Fraction of the Size
        this.bytes = function() {
            return bytesTotal % 1024;
        };
        this.kbytes = function() {
            return Math.floor(bytesTotal / bytesPerKb) % 1024;
        };
        this.mbytes = function() {
            return Math.floor(bytesTotal / bytesPerMb) % 1024;
        };
        this.gbytes = function() {
            return Math.floor(bytesTotal / bytesPerGb);
        };

        //this.toString = function () {
        //    var text = "";
        //    var negative = false;
        //    if (bytesTotal < 0) {
        //        negative = true;
        //        text += "-";
        //        bytesTotal = Math.abs(bytesTotal);
        //    }
        //    text += this.to2Digits(Math.floor(this.totalHours())) + ":" + this.to2Digits(this.gbytes());
        //    if (negative)
        //        bytesTotal *= -1;
        //    return text;
        //};
        //this.to2Digits = function (n) {
        //    if (n < 10)
        //        return "0" + n;
        //    return n;
        //};
    };

    // "Static Constructors"
    Size.FromBytes = function(bytes) {
        return new Size(bytes, 0, 0, 0);
    };
    Size.FromKBytes = function(kbytes) {
        return new Size(0, kbytes, 0, 0);
    };
    Size.FromMBytes = function(mbytes) {
        return new Size(0, 0, mbytes, 0);
    };
    Size.FromGBytes = function(gbytes) {
        return new Size(0, 0, 0, gbytes);
    };
}());