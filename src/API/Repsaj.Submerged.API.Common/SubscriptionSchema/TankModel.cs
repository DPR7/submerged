﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repsaj.Submerged.Common.SubscriptionSchema
{
    public class TankModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public TankModel(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }


        public static TankModel BuildTank(string name, string description)
        {
            TankModel model = new TankModel(name, description);
            model.Id = Guid.NewGuid();
            return model;
        }
    }
}