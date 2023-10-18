import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { InteractionStatus } from '@azure/msal-browser';
import { Subject, filter, takeUntil } from 'rxjs';
import { GRAPH_ENDPOINT } from '../app.module';

type ProfileType = {
  displayName?: string;
  givenName?: string;
  surname?: string;
  userPrincipalName?: string;
  id?: string;
};

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  private readonly _destroying$ = new Subject<void>();

  loggedIn = false;
  profile: ProfileType | undefined;

  constructor(
    private authService: MsalService,
    private broadcastService: MsalBroadcastService,
    private http: HttpClient
  ) {}

  get firstName(): string {
    return this.profile?.givenName ?? this.profile?.displayName ?? 'Anonymous';
  }

  ngOnInit() {
    this.broadcastService.inProgress$
      .pipe(
        filter((status: InteractionStatus) => status === InteractionStatus.None),
        takeUntil(this._destroying$)
      )
      .subscribe(() => {
        this.setLoginDisplay();
        this.loadProfile();
      });

    this.loadProfile();
  }

  login() {
    this.authService.loginRedirect();
  }

  setLoginDisplay() {
    if (this.authService.instance.getAllAccounts().length > 0) this.loggedIn = true;
  }

  loadProfile(): void {
    if (this.loggedIn && this.profile == undefined)
      this.http.get(GRAPH_ENDPOINT).subscribe((profile) => {
        this.profile = profile;
      });
  }

  logout() {
    this.authService.logoutRedirect({});
  }
}
