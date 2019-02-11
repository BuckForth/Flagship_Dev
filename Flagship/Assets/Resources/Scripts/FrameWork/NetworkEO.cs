using UnityEngine;
using System.Collections;

public static class NetworkEO
{
	public static string ServerTimeOut ()
	{
		return @"Disconected from server.
Maybe the host closed the lobby.";
	}

	public static string MasterServerTimeOut ()
	{
		return @"Unable to connect to network
Please check your internet connection.";
	}

	public static string ServerCantConnect ()
	{
		return @"Unable to connect to server.
Maybe the host closed the lobby.";
	}
		
}