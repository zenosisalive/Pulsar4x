﻿#region Copyright/License
/* 
 *Copyright© 2017 Daniel Phelps
    This file is part of Pulsar4x.

    Pulsar4x is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Pulsar4x is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Pulsar4x.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// Contains info on the hanger space in the ship.
    /// </summary>
    public class HangerDB : BaseDataBlob
    {
        #region Fields
        private int _hangerSpace;
        #endregion

        #region Properties
        /// <summary>
        /// Total amount of hanger space in the ship, in tons.
        /// </summary>
        public int HangerSpace { get { return _hangerSpace; } set { SetField(ref _hangerSpace, value); } }
        #endregion

        #region Constructors
        public HangerDB() { }

        public HangerDB(HangerDB hangerDB) { HangerSpace = hangerDB.HangerSpace; }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new HangerDB(this);
        #endregion
    }
}