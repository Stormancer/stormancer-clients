using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Stormancer
{
    /// <summary>
    /// Provides cached HTTP client instances.
    /// </summary>
    /// <remarks>
    /// DO Not dispose <see cref="HttpClient"/> objects provided by <see cref="CreateClient(string)"/>
    /// </remarks>
    public interface IHttpClientFactory
    {
        /// <summary>
        /// provides an http client instance.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        HttpClient CreateClient(string id);
    }
}
