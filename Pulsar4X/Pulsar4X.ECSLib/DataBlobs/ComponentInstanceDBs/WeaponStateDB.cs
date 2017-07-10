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

using System;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class WeaponStateDB : BaseDataBlob
    {
        #region Fields
        private TimeSpan _coolDown;

        [JsonProperty]
        private Entity _fireControl;
        #endregion

        #region Properties
        [JsonProperty]
        public TimeSpan CoolDown { get { return _coolDown; } set { SetField(ref _coolDown, value); } }


        public Entity FireControl
        {
            get { return _fireControl; }
            set
            {
                if (value == null)
                {
                    _fireControl = null;
                }
                else
                {
                    if (value.HasDataBlob<FireControlInstanceAbilityDB>())
                    {
                        _fireControl = value;
                    }
                    else
                    {
                        _fireControl = null;
                    }
                }
            }
        }
        #endregion

        #region Constructors
        public WeaponStateDB() { FireControl = null; }

        public WeaponStateDB(WeaponStateDB db)
        {
            CoolDown = db.CoolDown;
            FireControl = db.FireControl;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new WeaponStateDB(this);
        #endregion
    }
}