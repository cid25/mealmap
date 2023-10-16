import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  isIframe = window !== window.parent && !window.opener;

  ngOnInit(): void {
    this.isIframe = window !== window.parent && !window.opener;
  }
}
