/// <reference types="@angular/localize" />

import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';
import { InjectionToken } from '@angular/core';
import { IConfig } from './app/config';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

export const APP_CONFIG = new InjectionToken<IConfig>('app-config');

function configListener(this: XMLHttpRequest) {
  try {
    const configuration: IConfig = JSON.parse(this.responseText);

    // pass config to bootstrap process using an injection token
    platformBrowserDynamic([
      { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
      { provide: APP_CONFIG, useValue: configuration }
    ])
      .bootstrapModule(AppModule)
      .catch((err) => console.error(err));
  } catch (error) {
    console.error(error);
  }
}

function configFailed(evt: ProgressEvent<XMLHttpRequestEventTarget>) {
  console.error('Error: retrieving config.json', evt.timeStamp);
}

if (environment.production) {
  enableProdMode();
}

const request = new XMLHttpRequest();
request.addEventListener('load', configListener);
request.addEventListener('error', configFailed);
request.open('GET', '/api/settings');
request.send();
