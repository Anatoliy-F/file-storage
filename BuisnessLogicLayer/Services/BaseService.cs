using AutoMapper;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using BuisnessLogicLayer.Models;

namespace BuisnessLogicLayer.Services
{
    public abstract class BaseService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        protected BaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        // <summary>
        /// Pages, sorts and/or filters a IQueryable source
        /// </summary>
        /// <param name="source">An IQueryable source of generic type</param>
        /// <param name="PageIndex">Zero-based current page index (0 = first page)</param>
        /// <param name="PageSize">The actual size of each page</param>
        /// <param name="SortColumn">The sorting column name</param>
        /// <param name="SortOrder">The sorting order ("ASC" or "DESC")</param>
        /// <param name="FilterColumn">The filtering column name</param>
        /// <param name="FilterQuery">The filtering auery (value to lookup)</param>
        /// <returns>A object containing the IQueryable paged/sorted/filtered result
        /// and all the relevant paging/sorting/filtering navigation info</returns>
        protected async Task<ICollection<T>> TakePageFilteredAndOrderedAsync<T>
            (IQueryable<T> source, QueryModel query) where T : class, IEntity, new()
        {
            if (!string.IsNullOrEmpty(query.FilterColumn) && !string.IsNullOrEmpty(query.FilterQuery) && IsValidProperty<T>(query.FilterColumn))
            {
                source = source.Where(string.Format("{0}.StartsWith(@0)", query.FilterColumn), query.FilterQuery);
            }

            if (!string.IsNullOrEmpty(query.SortColumn) && IsValidProperty<T>(query.SortColumn))
            {
                query.SortOrder = !string.IsNullOrEmpty(query.SortOrder) && query.SortOrder.ToUpper() == "ASC"
                    ? "ASC" : "DESC";
                source = source.OrderBy(string.Format("{0} {1}", query.SortColumn, query.SortOrder));
            }

            source = source.Skip(query.PageIndex * query.PageSize).Take(query.PageSize);
            return await source.ToListAsync();
        }

        // <summary>
        /// Checks if the given property name exists to protect against SQL injection attacks
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="throwExceptionIfNotFound">if TRUE trhows exception if property doesn't exist</param>
        /// <returns>TRUE if property exists, FALSE otherwise</returns>
        /// <exception cref="NotSupportedException">Throws if property doesn't exist, and throwExceptionifNotFound set TRUE</exception>
        protected bool IsValidProperty<T>(string propertyName, bool throwExceptionIfNotFound = true) where T : class, IEntity, new()
        {
            var prop = typeof(T).GetProperty(propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null && throwExceptionIfNotFound)
            {
                throw new NotSupportedException($"Error: Property '{propertyName}' does not exist");
            }
            return prop != null;
        }
    }
}
