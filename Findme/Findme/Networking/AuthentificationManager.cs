﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Collections.Generic;
using System.ServiceModel;
using System.Net;

namespace Findme
{
	public class AuthentificationManager: NetworkingManager
	{

		public static AuthentificationManager singletonInstance;

		/// <summary>
		/// Gets the shared instance.
		/// </summary>
		/// <value>The shared instance.</value>
		public static AuthentificationManager SharedInstance
		{
			get 
			{
				if (singletonInstance == null)
				{
					singletonInstance = new AuthentificationManager ();
				}
				return singletonInstance;
			}
		}

		public AuthentificationManager ()
		{
		}

		/// <summary>
		/// Authentificates the user with facebook token.
		/// </summary>
		/// <returns>The user with facebook token.</returns>
		/// <param name="facebookToken">Facebook token.</param>
		public async Task<FindMeResponse> AuthentificateUserWithFacebookToken(String facebookToken)
		{
			var findMeResponse = new FindMeResponse ();
			SocialUser socialUser = await SocialAuthManager.sharedInstance.getFacebookSocialUserWithAuthToken(facebookToken);

			try
			{
				var parameters = new FormUrlEncodedContent(new[]
					{
						new KeyValuePair<string, string>("client-id", socialUser.id),
						new KeyValuePair<string, string>("client-token", socialUser.accessToken)
					}
				);

				var result = await client.PostAsync(NetworkingUrls.FACEBOOK_LOGIN_URL, parameters);
				findMeResponse = await NetworkingManager.getFindMeResponseFromHttpResponseMessage(result, "user");

				User user = JsonConvert.DeserializeObject<User>(findMeResponse.Result);
				if (null == user) {
					findMeResponse.ErrorInfo = "No Object Found";
				}
				else {
					UserStorage.SaveAccessAndRefreshToken(user.accessToken, user.refreshToken);
				}
			}
			catch (Exception ex)
			{
				ConsoleOutput.PrintLine ("Error message : " + ex.Message);
				findMeResponse.ErrorInfo = ex.Message;
			}

			return findMeResponse;
		}

		/// <summary>
		/// Authentificates the user.
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="email">Email.</param>
		/// <param name="password">Password.</param>
		public async Task<FindMeResponse> AuthentificateUser(String email, String password)
		{
			var findMeResponse = new FindMeResponse ();

			try
			{
				var parameters = new FormUrlEncodedContent(new[]
					{
						new KeyValuePair<string, string>("email", email),
						new KeyValuePair<string, string>("password", password)
					}
				);

				var result = await client.PostAsync(NetworkingUrls.LOGIN_URL, parameters);
				findMeResponse = await NetworkingManager.getFindMeResponseFromHttpResponseMessage(result, "user");

				User user = JsonConvert.DeserializeObject<User>(findMeResponse.Result);
				if (null == user) {
					findMeResponse.ErrorInfo = "No Object Found";
				}
				else {
					UserStorage.SaveAccessAndRefreshToken(user.accessToken, user.refreshToken);
				}
			}
			catch (Exception ex)
			{
				ConsoleOutput.PrintLine ("Error message : " + ex.Message);
				findMeResponse.ErrorInfo = ex.Message;
			}

			return findMeResponse;
		}

		/// <summary>
		/// Refreshs the token.
		/// </summary>
		/// <returns>The token.</returns>
		/// <param name="refreshTokenValue">Refresh token value.</param>
		public async Task<FindMeResponse> RefreshToken(String refreshTokenValue) {

			var findMeResponse = new FindMeResponse ();

			try
			{
				var parameters = new FormUrlEncodedContent(new[]
					{
						new KeyValuePair<string, string>("token", refreshTokenValue)
					}
				);

				var result = await client.PostAsync(NetworkingUrls.REFRESH_TOKEN_URL, parameters);
				findMeResponse = await NetworkingManager.getFindMeResponseFromHttpResponseMessage(result, "user");

				User user = JsonConvert.DeserializeObject<User>(findMeResponse.Result);
				if (null == user) {
					findMeResponse.ErrorInfo = "No Object Found";
				}
				else {
					UserStorage.SaveAccessAndRefreshToken(user.accessToken, user.refreshToken);
				}
			}
			catch (Exception ex)
			{
				ConsoleOutput.PrintLine ("Error message : " + ex.Message);
				findMeResponse.ErrorInfo = ex.Message;
			}

			return findMeResponse;
		}

		/// <summary>
		/// Registers the user.
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="email">Email.</param>
		/// <param name="password">Password.</param>
		/// <param name="firstName">First name.</param>
		/// <param name="lastName">Last name.</param>
		/// <param name="image">Image.</param>
		public async Task<FindMeResponse> RegisterUser(String email, String password, String firstName, String lastName, Image image)
		{
			var findMeResponse = new FindMeResponse ();

			try
			{
				var parameters = new FormUrlEncodedContent(new[]
					{
						new KeyValuePair<string, string>("email", email),
						new KeyValuePair<string, string>("password", password),
						new KeyValuePair<string, string>("firstname", firstName),
						new KeyValuePair<string, string>("lastname", lastName),
						new KeyValuePair<string, string>("image", "")
					}
				);

				var result = await client.PostAsync(NetworkingUrls.REGISTER_URL, parameters);
				findMeResponse = await NetworkingManager.getFindMeResponseFromHttpResponseMessage(result, "user");

				User user = JsonConvert.DeserializeObject<User>(findMeResponse.Result);
				if (null == user) {
					findMeResponse.ErrorInfo = "No Object Found";
				}
				else {
					UserStorage.SaveAccessAndRefreshToken(user.accessToken, user.refreshToken);
				}
			}
			catch (Exception ex)
			{
				ConsoleOutput.PrintLine ("Error message : " + ex.Message);
				findMeResponse.ErrorInfo = ex.Message;
			}

			return findMeResponse;
		}
	}
}
