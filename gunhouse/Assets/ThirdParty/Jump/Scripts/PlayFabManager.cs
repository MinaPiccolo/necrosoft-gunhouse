using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabManager : MonoBehaviour
{
    public string isReady;
	public string PlayFabId;
	Dictionary<string, UserDataRecord> playerData = new Dictionary<string, UserDataRecord>();
	List<StatisticValue> playerStats = new List<StatisticValue>();

	public void LoginToPlayFab(string userId)
	{
		Debug.Log ("logging in with Jump ID: " + userId);
		LoginWithCustomIDRequest request = new LoginWithCustomIDRequest { CustomId = userId, CreateAccount = true};
		PlayFabClientAPI.LoginWithCustomID(request, (result) => {
			PlayFabId = result.PlayFabId;
			Debug.Log("got PlayFabId: "+PlayFabId);

			if(result.NewlyCreated)
			{
				Debug.Log("(new account)");
			}
			else
			{
				Debug.Log("(existing account)");
			}

			GetPlayerData();//grab the newest data from the server for local access in the game
			GetPlayerStats();//grab the newest stats from the server for local access in the game
		},
		(error) => {
			Debug.Log("Error logging in player with Jump User ID:");
			Debug.Log(error.ErrorMessage);
		});
	}

	public void UpdatePlayerData(string key, string value) {
		UpdateUserDataRequest request = new UpdateUserDataRequest()
		{
			Data = new Dictionary<string,string>()
			{
				{key, value}
			}
		};

		PlayFabClientAPI.UpdateUserData(request,
			(result) =>
			{
				Debug.Log("Successfully updated user data");

				GetPlayerData();//grab the newest data from the server for local access in-game
			},
			(error) =>
			{
				Debug.Log("Got error setting user data");
				Debug.Log(error.ErrorMessage);
			});
	}

	public void GetPlayerData() {
		GetUserDataRequest request = new GetUserDataRequest()
		{
			PlayFabId = PlayFabId,
			Keys = null
		};
		PlayFabClientAPI.GetUserData(request,
			(result) => {
				Debug.Log("Got user data");

				if ((result.Data == null) || (result.Data.Count == 0)) {
					Debug.Log("No user data available");
				}
				else {
					playerData = result.Data;//update your local copy of playerData

					/*if you want to check that your data is coming in correctly, uncomment the loop below.*/
					//foreach (var item in result.Data)
					//{
					//	Debug.Log(item.Key + " = " + item.Value.Value);
					//}
				}

                Gunhouse.Platform.Initialize();
			},
			(error) => {
				Debug.Log("Got error retrieving user data" + error.ErrorMessage);
			});
	}

	public string GetDataValueForKey(string key) {
		string value = null;// = "empty";//we return "empty" if that data doesn't exist on the server for this player

        foreach(KeyValuePair<string, UserDataRecord> entry in playerData) {
			if (entry.Key == key) value = entry.Value.Value;
		}
		return value;
	}

	public void UpdatePlayerStats(string key, int value){
		print("requesting to update stat: " + key + " to " + value);
		UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest()
		{
			Statistics = new List<StatisticUpdate>
			{
				new StatisticUpdate{StatisticName = key, Value = value},
			}
		};

		PlayFabClientAPI.UpdatePlayerStatistics(request,
			(result) =>
			{
				Debug.Log("Successfully updated user stats: "+key+", "+value);
				GetPlayerStats();//grab the newest stats from the server for local use in-game
			},
			(error) =>
			{
				Debug.Log("Got error setting user stats: " + key + ", " + value);
				Debug.Log(error);
			});
	}

	public void GetPlayerStats()
	{
		GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest();
		PlayFabClientAPI.GetPlayerStatistics(request,
			(result) =>
			{
				Debug.Log("Got user stats");

				if ((result.Statistics == null) || (result.Statistics.Count == 0))
				{
					Debug.Log("No user stats available");
				}
				else
				{
					playerStats = result.Statistics;//update your local copy of playerStats
					//foreach (var item in result.UserStatistics)
					//{

					//	Debug.Log(item.Key + " = " + item.Value);
					//}
				}
			},
			(error) =>
			{
				Debug.Log("Got error retrieving user stats");
				Debug.Log(error.ErrorMessage);
			});
	}

	public int GetStatsValueForKey(string key)
	{
		int value = -1;//we return -1 if that stat doesn't exist on the server
		foreach (StatisticValue entry in playerStats)
		{
			if (entry.StatisticName == key) value = entry.Value;
		}
		return value;
	}

	public List<PlayerLeaderboardEntry> GetLeaderboard(string statName, int startPosition)
	{
		List<PlayerLeaderboardEntry> leaderboard = new List<PlayerLeaderboardEntry> ();
		GetLeaderboardRequest request = new GetLeaderboardRequest ()
		{
			StatisticName = statName,
			StartPosition = startPosition
		};
		PlayFabClientAPI.GetLeaderboard (request,
			(result) =>
			{
				Debug.Log("Got leaderboard for stat: "+statName);
				leaderboard = result.Leaderboard;
			},
			(error) =>
			{
				Debug.Log("Got error retrieving leaderboard");
				Debug.Log(error.ErrorMessage);
			});
		return leaderboard;
	}

	public List<PlayerLeaderboardEntry> GetLeaderboardAroundPlayer(string statName)
	{
		List<PlayerLeaderboardEntry> leaderboard = new List<PlayerLeaderboardEntry> ();
		GetLeaderboardAroundPlayerRequest request = new GetLeaderboardAroundPlayerRequest ()
		{
			PlayFabId = PlayFabId,
			StatisticName = statName
		};
		PlayFabClientAPI.GetLeaderboardAroundPlayer (request,
			(result) =>
			{
				Debug.Log("Got leaderboard around player for stat: "+statName);
				leaderboard = result.Leaderboard;
			},
			(error) =>
			{
				Debug.Log("Got error retrieving leaderboard around player");
				Debug.Log(error.ErrorMessage);
			});
		return leaderboard;
	}
}
