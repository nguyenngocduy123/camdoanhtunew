﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CamDoAnhTu.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class CamdoAnhTuEntities1 : DbContext
    {
        public CamdoAnhTuEntities1()
            : base("name=CamdoAnhTuEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<history> histories { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Loan> Loans { get; set; }
        public virtual DbSet<User> Users { get; set; }
    
        public virtual ObjectResult<GetCustomerEven_Result> GetCustomerEven()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetCustomerEven_Result>("GetCustomerEven");
        }
    
        public virtual ObjectResult<GetCustomerOdd_Result> GetCustomerOdd()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetCustomerOdd_Result>("GetCustomerOdd");
        }
    
        public virtual ObjectResult<Nullable<decimal>> GetTienGoc(Nullable<int> type)
        {
            var typeParameter = type.HasValue ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("GetTienGoc", typeParameter);
        }
    
        public virtual ObjectResult<Nullable<decimal>> GetTienLai(Nullable<int> type)
        {
            var typeParameter = type.HasValue ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("GetTienLai", typeParameter);
        }
    
        public virtual ObjectResult<Nullable<decimal>> GetTienLaiThatTe(Nullable<int> type, Nullable<System.DateTime> date1, Nullable<System.DateTime> date2)
        {
            var typeParameter = type.HasValue ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(int));
    
            var date1Parameter = date1.HasValue ?
                new ObjectParameter("date1", date1) :
                new ObjectParameter("date1", typeof(System.DateTime));
    
            var date2Parameter = date2.HasValue ?
                new ObjectParameter("date2", date2) :
                new ObjectParameter("date2", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("GetTienLaiThatTe", typeParameter, date1Parameter, date2Parameter);
        }
    
        public virtual int SumMoneyByCode(Nullable<System.DateTime> datetimeinput, Nullable<int> type)
        {
            var datetimeinputParameter = datetimeinput.HasValue ?
                new ObjectParameter("datetimeinput", datetimeinput) :
                new ObjectParameter("datetimeinput", typeof(System.DateTime));
    
            var typeParameter = type.HasValue ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SumMoneyByCode", datetimeinputParameter, typeParameter);
        }
    
        public virtual ObjectResult<Nullable<decimal>> GetTienGoc_Dung(Nullable<int> type)
        {
            var typeParameter = type.HasValue ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("GetTienGoc_Dung", typeParameter);
        }
    
        public virtual ObjectResult<Nullable<decimal>> GetTienLaiThatTe_Dung(Nullable<int> type, Nullable<System.DateTime> date1, Nullable<System.DateTime> date2)
        {
            var typeParameter = type.HasValue ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(int));
    
            var date1Parameter = date1.HasValue ?
                new ObjectParameter("date1", date1) :
                new ObjectParameter("date1", typeof(System.DateTime));
    
            var date2Parameter = date2.HasValue ?
                new ObjectParameter("date2", date2) :
                new ObjectParameter("date2", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("GetTienLaiThatTe_Dung", typeParameter, date1Parameter, date2Parameter);
        }
    }
}
