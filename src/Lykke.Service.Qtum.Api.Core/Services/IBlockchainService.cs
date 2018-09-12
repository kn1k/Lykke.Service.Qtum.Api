﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs;
using NBitcoin;

namespace Lykke.Service.Qtum.Api.Core.Services
{
    /// <summary>
    /// Blockchain service interface
    /// </summary>
    public interface IBlockchainService
    {
        /// <summary>
        /// Get network
        /// </summary>
        /// <returns>Blockchain network <see cref="Network"/></returns>
        Network GetNetwork();
        
        /// <summary>
        /// Address validate
        /// </summary>
        /// <param name="address">Blockchain address</param>
        /// <returns>Is assress valid</returns>
        bool IsAddressValid(string address);
        
        /// <summary>
        /// Bitcoin address parse
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Parsed address</returns>
        BitcoinAddress ParseAddress(string address);

        /// <summary>
        /// Address validate
        /// </summary>
        /// <param name="address">Blockchain address</param>
        /// <param name="exception">Validate exception</param>
        /// <returns>Is assress valid</returns>
        bool IsAddressValid(string address, out Exception exception);
        
        /// <summary>
        /// Get address chain height
        /// </summary>
        /// <returns>Block count</returns>
        Task<Int64> GetBlockCountAsync();
        
        /// <summary>
        /// Get balance for address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Balance for address</returns>
        Task<BigInteger> GetAddressBalanceAsync(BitcoinAddress address);

        /// <summary>
        /// Get transactions info for specified address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>List of transaction info items</returns>
        Task<List<IItem>> GetAddressTransactionsInfoAsync(BitcoinAddress address);
    }
    
    public enum TransactionType
    {
        open,
        receive,
        send,
        change
    }
}
