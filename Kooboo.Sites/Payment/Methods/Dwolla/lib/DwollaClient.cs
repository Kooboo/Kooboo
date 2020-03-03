using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
//using Dwolla.Client.Models;
//using Dwolla.Client.Models.Requests;
//using Dwolla.Client.Models.Responses;
//using Dwolla.Client.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

[assembly: InternalsVisibleTo("Dwolla.Client.Tests")]

namespace Dwolla.Client
{
    //public interface IDwollaClient
    //{
    //    string ApiBaseAddress { get; }
    //    string AuthBaseAddress { get; }

    //    Task<RestResponse<TRes>> PostAuthAsync<TRes>(Uri uri, AppTokenRequest content) where TRes : IDwollaResponse;
    //    Task<RestResponse<TRes>> GetAsync<TRes>(Uri uri, Headers headers) where TRes : IDwollaResponse;
    //    Task<RestResponse<TRes>> PostAsync<TReq, TRes>(Uri uri, TReq content, Headers headers) where TRes : IDwollaResponse;
    //    Task<RestResponse<EmptyResponse>> DeleteAsync<TReq>(Uri uri, TReq content, Headers headers);
    //    Task<RestResponse<EmptyResponse>> UploadAsync(Uri uri, UploadDocumentRequest content, Headers headers);
    //}

    //public class DwollaClient : IDwollaClient
    //{
    //    public const string ContentType = "application/vnd.dwolla.v1.hal+json";
    //    public string ApiBaseAddress { get; }
    //    public string AuthBaseAddress { get; }

    //    private static readonly JsonSerializerSettings JsonSettings =
    //        new JsonSerializerSettings
    //        {
    //            ContractResolver = new CamelCasePropertyNamesContractResolver(),
    //            NullValueHandling = NullValueHandling.Ignore
    //        };

    //    private readonly IRestClient _client;

    //    public static DwollaClient Create(bool isSandbox) =>
    //        new DwollaClient(new RestClient(CreateHttpClient()), isSandbox);

    //    public async Task<RestResponse<TRes>> PostAuthAsync<TRes>(
    //        Uri uri, AppTokenRequest content) where TRes : IDwollaResponse =>
    //        await SendAsync<TRes>(new HttpRequestMessage(HttpMethod.Post, uri)
    //        {
    //            Content = new FormUrlEncodedContent(new Dictionary<string, string>
    //            {
    //                {"client_id", content.Key}, {"client_secret", content.Secret}, {"grant_type", content.GrantType}
    //            })
    //        });

    //    public async Task<RestResponse<TRes>> GetAsync<TRes>(
    //        Uri uri, Headers headers) where TRes : IDwollaResponse =>
    //        await SendAsync<TRes>(CreateRequest(HttpMethod.Get, uri, headers));

    //    public async Task<RestResponse<TRes>> PostAsync<TReq, TRes>(
    //        Uri uri, TReq content, Headers headers) where TRes : IDwollaResponse =>
    //        await SendAsync<TRes>(CreatePostRequest(uri, content, headers));

    //    public async Task<RestResponse<EmptyResponse>> UploadAsync(
    //        Uri uri, UploadDocumentRequest content, Headers headers) =>
    //        await SendAsync<EmptyResponse>(CreateUploadRequest(uri, content, headers));

    //    public async Task<RestResponse<EmptyResponse>> DeleteAsync<TReq>(Uri uri, TReq content, Headers headers) =>
    //        await SendAsync<EmptyResponse>(CreateDeleteRequest(uri, content, headers));

    //    private async Task<RestResponse<TRes>> SendAsync<TRes>(HttpRequestMessage request) =>
    //        await _client.SendAsync<TRes>(request);

    //    private static HttpRequestMessage CreateDeleteRequest<TReq>(
    //        Uri requestUri, TReq content, Headers headers, string contentType = ContentType) =>
    //        CreateContentRequest(HttpMethod.Delete, requestUri, headers, content, contentType);

    //    private static HttpRequestMessage CreatePostRequest<TReq>(
    //        Uri requestUri, TReq content, Headers headers, string contentType = ContentType) =>
    //        CreateContentRequest(HttpMethod.Post, requestUri, headers, content, contentType);

    //    private static HttpRequestMessage CreateContentRequest<TReq>(
    //        HttpMethod method, Uri requestUri, Headers headers, TReq content, string contentType)
    //    {
    //        var r = CreateRequest(method, requestUri, headers);
    //        r.Content = new StringContent(JsonConvert.SerializeObject(content, JsonSettings), Encoding.UTF8, contentType);
    //        return r;
    //    }

    //    private static HttpRequestMessage CreateUploadRequest(Uri requestUri, UploadDocumentRequest content,
    //        Headers headers)
    //    {
    //        var r = CreateRequest(HttpMethod.Post, requestUri, headers);
    //        r.Content = new MultipartFormDataContent("----------Upload")
    //        {
    //            {new StringContent(content.DocumentType), "\"documentType\""},
    //            GetFileContent(content.Document)
    //        };
    //        return r;
    //    }

    //    private static StreamContent GetFileContent(File file)
    //    {
    //        var fc = new StreamContent(file.Stream);
    //        fc.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
    //        fc.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
    //        {
    //            Name = "\"file\"",
    //            FileName = $"\"{file.Filename}\""
    //        };
    //        return fc;
    //    }

    //    private static HttpRequestMessage CreateRequest(HttpMethod method, Uri requestUri, Headers headers)
    //    {
    //        var r = new HttpRequestMessage(method, requestUri);
    //        r.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
    //        foreach (var header in headers) r.Headers.Add(header.Key, header.Value);
    //        return r;
    //    }

    //    internal DwollaClient(IRestClient client, bool isSandbox)
    //    {
    //        _client = client;
    //        ApiBaseAddress = isSandbox ? "https://api-sandbox.dwolla.com" : "https://api.dwolla.com";
    //        AuthBaseAddress = isSandbox ? "https://accounts-sandbox.dwolla.com" : "https://accounts.dwolla.com";
    //    }

    //    private static readonly string ClientVersion = typeof(DwollaClient).GetTypeInfo().Assembly
    //        .GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

    //    internal static HttpClient CreateHttpClient()
    //    {
    //        var client = new HttpClient(new HttpClientHandler { SslProtocols = SslProtocols.Tls12 });
    //        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("dwolla-v2-csharp", ClientVersion));
    //        return client;
    //    }
    //}
}