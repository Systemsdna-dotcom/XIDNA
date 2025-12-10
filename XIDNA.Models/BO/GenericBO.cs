using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XIDNA.Models
{
    public class GenericBO<T> where T : class
    {
        ModelDbContext dbContext = new ModelDbContext();
        private IDbSet<T> entities;

        public GenericBO(ModelDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        private IDbSet<T> Entities
        {
            get
            {
                if (entities == null)
                {
                    entities = dbContext.Set<T>();
                }
                return entities;
            }
        }


        public T Insert_Data(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("entity");
            }
            this.Entities.Add(model);
            this.dbContext.SaveChanges();
            return model;
        }

        public T Update_Data(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("entity");
            }
            this.dbContext.SaveChanges();
            return model;
        }

    }
}
