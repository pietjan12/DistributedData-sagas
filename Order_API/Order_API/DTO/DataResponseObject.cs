using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_API.DTO
{
    public class DataResponseObject<T> where T : class
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        /// <summary>
        /// Primarily used for empty responses that still need to indicate success
        /// </summary>
        /// <param name="success"></param>
        public DataResponseObject(bool success)
        {
            Success = success;
        }

        /// <summary>
        /// used when errors occured within the operation
        /// </summary>
        /// <param name="message">the errormessage</param>
        public DataResponseObject(string message)
        {
            Success = false;
            Message = message;
        }

        /// <summary>
        /// Used when operation was completed successfully and you wish to return data to the user
        /// </summary>
        /// <param name="data">the data to return</param>
        public DataResponseObject(T data)
        {
            Success = true;
            Data = data;
        }
    }
}
