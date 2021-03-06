﻿using Repsaj.Submerged.GatewayApp.Universal.Models.ConfigurationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Repsaj.Submerged.GatewayApp.Universal.Control.LED
{
    public class RGBWValue : IEqualityComparer<RGBWValue>, IEquatable<RGBWValue>
    {
        public byte R = 0;
        public byte G = 0;
        public byte B = 0;
        public byte W = 0;

        public RGBWValue()
        {

        }
        public RGBWValue(byte r, byte g, byte b, byte w)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.W = w;
        }

        public bool NoOutput()
        {
            return R == 0 &&
                   G == 0 &&
                   B == 0 &&
                   W == 0;
        }

        public override String ToString()
        {
            return $"RGBW({R}, {G}, {B}, {W})";
        }

        #region Equality checking
        public override bool Equals(object obj)
        {
            if (!(obj is RGBWValue))
                return false;
            else
            {
                RGBWValue other = (RGBWValue)obj;
                return Equals(this, other);
            }
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public bool Equals(RGBWValue x, RGBWValue y)
        {
            return x.R == y.R &&
                   x.G == y.G &&
                   x.B == y.B &&
                   x.W == y.W;
        }

        public int GetHashCode(RGBWValue obj)
        {
            string hashString = string.Format($"{obj.R}{obj.G}{obj.B}{obj.W}");
            return hashString.GetHashCode();
        }

        public bool Equals(RGBWValue other)
        {
            return this.R == other.R &&
                   this.G == other.G &&
                   this.B == other.B &&
                   this.W == other.W;
        }
        #endregion
    }
}
