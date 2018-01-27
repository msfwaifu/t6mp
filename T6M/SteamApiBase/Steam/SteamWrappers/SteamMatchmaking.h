#pragma once
class SteamMatchmaking009 : public ISteamMatchmaking009
{
public:
	int GetFavoriteGameCount();

	bool GetFavoriteGame(int iGame, AppId_t *pnAppID, uint32 *pnIP, uint16 *pnConnPort, uint16 *pnQueryPort, uint32 *punFlags, uint32 *pRTime32LastPlayedOnServer);

	int AddFavoriteGame(AppId_t nAppID, uint32 nIP, uint16 nConnPort, uint16 nQueryPort, uint32 unFlags, uint32 rTime32LastPlayedOnServer);

	bool RemoveFavoriteGame(AppId_t nAppID, uint32 nIP, uint16 nConnPort, uint16 nQueryPort, uint32 unFlags);

	SteamAPICall_t RequestLobbyList();
	void AddRequestLobbyListStringFilter(const char *pchKeyToMatch, const char *pchValueToMatch, ELobbyComparison eComparisonType);
	void AddRequestLobbyListNumericalFilter(const char *pchKeyToMatch, int nValueToMatch, ELobbyComparison eComparisonType);
	void AddRequestLobbyListNearValueFilter(const char *pchKeyToMatch, int nValueToBeCloseTo);
	void AddRequestLobbyListFilterSlotsAvailable(int nSlotsAvailable);
	void AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter eLobbyDistanceFilter);
	void AddRequestLobbyListResultCountFilter(int cMaxResults);

	void AddRequestLobbyListCompatibleMembersFilter(CSteamID steamID);

	CSteamID GetLobbyByIndex(int iLobby);
	SteamAPICall_t CreateLobby(ELobbyType eLobbyType, int cMaxMembers);

	SteamAPICall_t JoinLobby(CSteamID steamIDLobby);

	void LeaveLobby(CSteamID steamIDLobby);

	bool InviteUserToLobby(CSteamID steamIDLobby, CSteamID steamIDInvitee);

	int GetNumLobbyMembers(CSteamID steamIDLobby);
	CSteamID GetLobbyMemberByIndex(CSteamID steamIDLobby, int iMember);
	const char *GetLobbyData(CSteamID steamIDLobby, const char *pchKey);
	bool SetLobbyData(CSteamID steamIDLobby, const char *pchKey, const char *pchValue);

	int GetLobbyDataCount(CSteamID steamIDLobby);

	bool GetLobbyDataByIndex(CSteamID steamIDLobby, int iLobbyData, char *pchKey, int cchKeyBufferSize, char *pchValue, int cchValueBufferSize);

	bool DeleteLobbyData(CSteamID steamIDLobby, const char *pchKey);

	const char *GetLobbyMemberData(CSteamID steamIDLobby, CSteamID steamIDUser, const char *pchKey);
	void SetLobbyMemberData(CSteamID steamIDLobby, const char *pchKey, const char *pchValue);

	bool SendLobbyChatMsg(CSteamID steamIDLobby, const void *pvMsgBody, int cubMsgBody);
	int GetLobbyChatEntry(CSteamID steamIDLobby, int iChatID, CSteamID *pSteamIDUser, void *pvData, int cubData, EChatEntryType *peChatEntryType);

	bool RequestLobbyData(CSteamID steamIDLobby);

	void SetLobbyGameServer(CSteamID steamIDLobby, uint32 unGameServerIP, uint16 unGameServerPort, CSteamID steamIDGameServer);
	bool GetLobbyGameServer(CSteamID steamIDLobby, uint32 *punGameServerIP, uint16 *punGameServerPort, CSteamID *psteamIDGameServer);

	bool SetLobbyMemberLimit(CSteamID steamIDLobby, int cMaxMembers);
	int GetLobbyMemberLimit(CSteamID steamIDLobby);

	bool SetLobbyType(CSteamID steamIDLobby, ELobbyType eLobbyType);

	bool SetLobbyJoinable(CSteamID steamIDLobby, bool bLobbyJoinable);

	CSteamID GetLobbyOwner(CSteamID steamIDLobby);
	bool SetLobbyOwner(CSteamID steamIDLobby, CSteamID steamIDNewOwner);

	bool SetLinkedLobby(CSteamID steamIDLobby, CSteamID steamIDLobby2);
};