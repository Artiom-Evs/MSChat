class ServerSideConfig {
  private _oidcServerUri: string = "";
  private _oidcClientId: string = "";
  private _chatApiUri: string = "";

  get oidcServerUri() {
    if (!this._oidcServerUri) {
      throw new Error(`"oidcServerUri" is not configured.`);
    }

    return this._oidcServerUri;
  }

  get oidcClientId() {
    if (!this._oidcClientId) {
      throw new Error(`"oidcClientId" is not configured.`);
    }

    return this._oidcClientId;
  }

  get chatApiUri() {
    if (!this._chatApiUri) {
      throw new Error(`"chatApiUri" is not configured.`);
    }

    return this._chatApiUri;
  }

  public async init(): Promise<void> {
    try {
      // get server-side app configuration
      const response = await fetch("/app-config");
      const data = await response.json();

      this._oidcServerUri = data.oidcServerUri;
      this._oidcClientId = data.oidcClientId;
      this._chatApiUri = data.chatApiUri;

      // validate configuration
      this.oidcServerUri;
      this.oidcClientId;
      this.chatApiUri;
    } catch (e: any) {
      console.error("Error while load app configuration:", e);
    }
  }
}

export const config = new ServerSideConfig();
