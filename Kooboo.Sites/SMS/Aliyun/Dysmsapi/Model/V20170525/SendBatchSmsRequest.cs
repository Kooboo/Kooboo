/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
using System.Collections.Generic;

using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Transform;
using Aliyun.Acs.Core.Utils;
using Aliyun.Acs.Dysmsapi.Transform;
using Aliyun.Acs.Dysmsapi.Transform.V20170525;

namespace Aliyun.Acs.Dysmsapi.Model.V20170525
{
    public class SendBatchSmsRequest : RpcAcsRequest<SendBatchSmsResponse>
    {
        public SendBatchSmsRequest()
            : base("Dysmsapi", "2017-05-25", "SendBatchSms", "dysms", "openAPI")
        {
            if (this.GetType().GetProperty("ProductEndpointMap") != null && this.GetType().GetProperty("ProductEndpointType") != null)
            {
                this.GetType().GetProperty("ProductEndpointMap").SetValue(this, Endpoint.endpointMap, null);
                this.GetType().GetProperty("ProductEndpointType").SetValue(this, Endpoint.endpointRegionalType, null);
            }
        }

		private long? resourceOwnerId;

		private string templateParamJson;

		private string resourceOwnerAccount;

		private long? ownerId;

		private string smsUpExtendCodeJson;

		private string signNameJson;

		private string templateCode;

		private string phoneNumberJson;

		public long? ResourceOwnerId
		{
			get
			{
				return resourceOwnerId;
			}
			set	
			{
				resourceOwnerId = value;
				DictionaryUtil.Add(QueryParameters, "ResourceOwnerId", value.ToString());
			}
		}

		public string TemplateParamJson
		{
			get
			{
				return templateParamJson;
			}
			set	
			{
				templateParamJson = value;
				DictionaryUtil.Add(QueryParameters, "TemplateParamJson", value);
			}
		}

		public string ResourceOwnerAccount
		{
			get
			{
				return resourceOwnerAccount;
			}
			set	
			{
				resourceOwnerAccount = value;
				DictionaryUtil.Add(QueryParameters, "ResourceOwnerAccount", value);
			}
		}

		public long? OwnerId
		{
			get
			{
				return ownerId;
			}
			set	
			{
				ownerId = value;
				DictionaryUtil.Add(QueryParameters, "OwnerId", value.ToString());
			}
		}

		public string SmsUpExtendCodeJson
		{
			get
			{
				return smsUpExtendCodeJson;
			}
			set	
			{
				smsUpExtendCodeJson = value;
				DictionaryUtil.Add(QueryParameters, "SmsUpExtendCodeJson", value);
			}
		}

		public string SignNameJson
		{
			get
			{
				return signNameJson;
			}
			set	
			{
				signNameJson = value;
				DictionaryUtil.Add(QueryParameters, "SignNameJson", value);
			}
		}

		public string TemplateCode
		{
			get
			{
				return templateCode;
			}
			set	
			{
				templateCode = value;
				DictionaryUtil.Add(QueryParameters, "TemplateCode", value);
			}
		}

		public string PhoneNumberJson
		{
			get
			{
				return phoneNumberJson;
			}
			set	
			{
				phoneNumberJson = value;
				DictionaryUtil.Add(QueryParameters, "PhoneNumberJson", value);
			}
		}

        public override SendBatchSmsResponse GetResponse(UnmarshallerContext unmarshallerContext)
        {
            return SendBatchSmsResponseUnmarshaller.Unmarshall(unmarshallerContext);
        }
    }
}
