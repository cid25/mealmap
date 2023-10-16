import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import routes from './routes';
import { BrowserUtils } from '@azure/msal-browser';

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      // Don't perform initial navigation in iframes or popups
      initialNavigation:
        !BrowserUtils.isInIframe() && !BrowserUtils.isInPopup() ? 'enabledNonBlocking' : 'disabled'
    })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {}
