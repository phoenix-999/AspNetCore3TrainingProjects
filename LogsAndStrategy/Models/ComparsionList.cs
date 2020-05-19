using Microsoft.AspNetCore.Server.IIS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class ComparsionList<T> : List<T>
    {
        public ComparsionList()
        {
            
        }

        public ComparsionList(List<T> list)
        {
            AddRange(list);
        }

        private bool Equal(List<T> other)
        {
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.Count != other.Count)
                return false;

            for(int i = 0; i < this.Count; i++)
            {
                var obj = this[i];
                var otherObj = other[i];

                Type objType = obj.GetType();
                var objProperties = objType.GetProperties();
                foreach(var property in objProperties)
                {
                    var thisValue = property.GetValue(obj);
                    var otherValue = property.GetValue(otherObj);
                    if (thisValue != otherValue)
                        return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is List<T> other)
                return Equal(other);
            else
                return false;
        }

        public override int GetHashCode()
        {
            //string allProps = "";
            //for (int i = 0; i < this.Count; i++)
            //{
            //    var obj = this[i];
            //    Type objType = obj.GetType();
            //    var objProperties = objType.GetProperties();
            //    foreach (var property in objProperties)
            //    {
            //        var thisValue = property.GetValue(obj);
            //        allProps += thisValue.ToString();
            //    }
            //}
            //return allProps.GetHashCode();
            return base.GetHashCode();
        }

        public static bool operator ==(ComparsionList<T> objects, List<T> other)
        {
            return objects.Equal(other);
        }
        public static bool operator !=(ComparsionList<T> objects, List<T> other)
        {
            return !objects.Equal(other);
        }
    }
}
