﻿// Sharlayan ~ ZoneHelper.cs
// 
// Copyright © 2007 - 2017 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Concurrent;
using System.Linq;
using Sharlayan.Models;

namespace Sharlayan.Helpers
{
    public static class ZoneHelper
    {
        private static bool Loading;

        private static MapItem DefaultMapItem = new MapItem
        {
            Name = new Localization
            {
                Chinese = "???",
                English = "???",
                French = "???",
                German = "???",
                Japanese = "???",
                Korean = "???"
            },
            Index = 0,
            IsDungeonInstance = false
        };

        private static ConcurrentDictionary<uint, MapItem> _mapInfos;

        private static ConcurrentDictionary<uint, MapItem> MapInfos
        {
            get { return _mapInfos ?? (_mapInfos = new ConcurrentDictionary<uint, MapItem>()); }
            set
            {
                if (_mapInfos == null)
                {
                    _mapInfos = new ConcurrentDictionary<uint, MapItem>();
                }
                _mapInfos = value;
            }
        }

        public static MapItem MapInfo(uint id)
        {
            if (Loading)
            {
                return DefaultMapItem;
            }
            lock (MapInfos)
            {
                if (MapInfos.Any())
                {
                    return MapInfos.ContainsKey(id) ? MapInfos[id] : DefaultMapItem;
                }
                Resolve();
                return DefaultMapItem;
            }
        }

        internal static void Resolve()
        {
            if (Loading)
            {
                return;
            }
            Loading = true;
            APIHelper.GetZones(MapInfos);
            Loading = false;
        }
    }
}
