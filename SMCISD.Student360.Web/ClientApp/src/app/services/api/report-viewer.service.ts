import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ReportViewerService {
  private viewerUrl = "";

    constructor() { }

  public updateUrl(url: string) {
      this.viewerUrl = url;
  }
  public getUrl(): string { return this.viewerUrl}
}