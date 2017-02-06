using System;
using Microsoft.AspNet.SignalR;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.SignalR;

// Copyright 2014 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.SignalR() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationSignalRExtensions
    {
        /// <summary>
        /// Adds a sink that writes log events as documents to a SignalR hub. 
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="context">The hub context.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="groupNames">Names of the Signalr groups you are broadcasting the log event to. Default is All Groups.</param>
        /// <param name="userIds">ID's of the Signalr Users you are broadcasting the log event to. Default is All Users.</param>
        /// <param name="excludedConnectionIds">Signalr connection ID's to exclude from broadcast.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration SignalR(
            this LoggerSinkConfiguration loggerConfiguration,
            IHubContext context,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = SignalRSink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null,
            string[] groupNames = null,
            string[] userIds = null,
            string[] excludedConnectionIds = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (context == null) throw new ArgumentNullException(nameof(context));

            var defaultedPeriod = period ?? SignalRSink.DefaultPeriod;
            return loggerConfiguration.Sink(
                new SignalRSink(context, batchPostingLimit, defaultedPeriod, formatProvider, groupNames, userIds, excludedConnectionIds),
                restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a sink that writes log events as documents to a SignalR hub. 
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="url">The url of the hub. http://localhost:8080.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="hub">The name of the Signalr hub class. Default is LogHub</param>
        /// <param name="groupNames">Names of the Signalr groups you are broadcasting the log event to. Default is All Groups.</param>
        /// <param name="userIds">ID's of the Signalr Users you are broadcasting the log event to. Default is All Users.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration SignalRClient(
            this LoggerSinkConfiguration loggerConfiguration,
            string url,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = SignalRSink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null,
            string hub = "LogHub",
            string[] groupNames = null,
            string[] userIds = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (url == null) throw new ArgumentNullException(nameof(url));

            var defaultedPeriod = period ?? SignalRSink.DefaultPeriod;
            return loggerConfiguration.Sink(
                new SignalRClientSink(url, batchPostingLimit, defaultedPeriod, formatProvider, hub, groupNames, userIds),
                restrictedToMinimumLevel);
        }
    }
}
