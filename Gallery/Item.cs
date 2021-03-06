﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery
{
   public class Item
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }


         public Item(string name,Uri uri)
        {
            Name = name;
            ImagePath = uri.ToString();
        }


        public override bool Equals(object obj)
        {
            var item = obj as Item;

            if (item == null)
            {
                return false;
            }

            return this.Name.Equals(item.Name);

        }
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }


    
}
