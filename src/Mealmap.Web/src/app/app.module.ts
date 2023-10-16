import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule, inject } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import {
  MSAL_INSTANCE,
  MSAL_INTERCEPTOR_CONFIG,
  MsalBroadcastService,
  MsalInterceptor,
  MsalInterceptorConfiguration,
  MsalModule,
  MsalRedirectComponent,
  MsalService
} from '@azure/msal-angular';
import {
  BrowserCacheLocation,
  IPublicClientApplication,
  InteractionType,
  PublicClientApplication
} from '@azure/msal-browser';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DishCardComponent } from './dish-card/dish-card.component';
import { DishEditorComponent } from './dish-editor/dish-editor.component';
import { DishOverviewComponent } from './dish-overview/dish-overview.component';
import { DishPickerComponent } from './dish-picker/dish-picker.component';
import { DishViewerComponent } from './dish-viewer/dish-viewer.component';
import { MealCardComponent } from './meal-card/meal-card.component';
import { MealEditorComponent } from './meal-editor/meal-editor.component';
import { MealScheduleComponent } from './meal-schedule/meal-schedule.component';
import { MealViewerComponent } from './meal-viewer/meal-viewer.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { SearchInputComponent } from './search-input/search-input.component';
import { APP_CONFIG } from 'src/main';

export function MSALInstanceFactory(): IPublicClientApplication {
  const config = inject(APP_CONFIG);

  return new PublicClientApplication({
    auth: {
      clientId: config.clientId,
      authority: config.authority,
      redirectUri: config.redirectUri
    },
    cache: {
      cacheLocation: BrowserCacheLocation.LocalStorage,
      storeAuthStateInCookie: true // set to true for IE 11
    }
  });
}

export function MSALInterceptorConfigFactory(): MsalInterceptorConfiguration {
  const config = inject(APP_CONFIG);

  const protectedResourceMap = new Map<string, Array<string>>();
  protectedResourceMap.set('api/*', [config.apiScope]);

  return {
    interactionType: InteractionType.Redirect,
    protectedResourceMap
  };
}

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    MealScheduleComponent,
    MealCardComponent,
    MealEditorComponent,
    DishPickerComponent,
    DishOverviewComponent,
    DishCardComponent,
    DishEditorComponent,
    DishViewerComponent,
    MealViewerComponent,
    SearchInputComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    NgbModule,
    MsalModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS, // Provides as HTTP Interceptor
      useClass: MsalInterceptor,
      multi: true
    },
    {
      provide: MSAL_INSTANCE,
      useFactory: MSALInstanceFactory
    },
    {
      provide: MSAL_INTERCEPTOR_CONFIG,
      useFactory: MSALInterceptorConfigFactory
    },
    MsalService,
    MsalBroadcastService
  ],
  bootstrap: [AppComponent, MsalRedirectComponent]
})
export class AppModule {}
