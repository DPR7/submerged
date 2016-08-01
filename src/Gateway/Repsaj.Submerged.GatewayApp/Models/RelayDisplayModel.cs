﻿using Repsaj.Submerged.GatewayApp.Universal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repsaj.Submerged.GatewayApp.Models
{
    public class RelayDisplayModel : NotificationBase<Relay>
    {
        public RelayDisplayModel(Relay relay = null) : base(relay) { }

        public String Name
        {
            get { return This.Name; }
            set { SetProperty(This.Name, value, () => This.Name = value); }
        }

        public bool? State
        {
            get { return This.State; }
            set { SetProperty(This.State, value, () => This.State = value); this.RaisePropertyChanged(nameof(RelayStateAsText)); }
        }

        public int? OrderNumber
        {
            get { return This.OrderNumber; }
            set { SetProperty(This.OrderNumber, value, () => This.OrderNumber = value); }
        }

        public string RelayStateAsText
        {
            get
            {
                if (this.State.HasValue)
                    return this.State.Value ? "ON" : "OFF";
                else
                    return String.Empty;
            }
        }
    }
}