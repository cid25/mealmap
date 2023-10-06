import {
  AfterViewInit,
  Component,
  ElementRef,
  Output,
  ViewChild,
  EventEmitter
} from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { timer } from 'rxjs';

@Component({
  selector: 'app-search-input',
  templateUrl: './search-input.component.html'
})
export class SearchInputComponent implements AfterViewInit {
  private lastSearch: string = '';

  @ViewChild('search')
  search: ElementRef | undefined;

  @Output()
  searched = new EventEmitter();

  form = new FormGroup({
    search: new FormControl<string>('')
  });

  ngAfterViewInit(): void {
    this.search?.nativeElement.focus();
  }

  async onSearchInput(): Promise<void> {
    const searchterm = this.form.controls.search.value?.trim().toLowerCase();

    if (
      searchterm != undefined &&
      (searchterm!.length >= 2 || (searchterm == '' && this.lastSearch != ''))
    ) {
      this.lastSearch = searchterm;
      timer(800).subscribe(() => {
        if (searchterm == this.lastSearch) this.searched.emit({ searchterm: searchterm });
      });
    }
  }

  async onClickSearch() {
    const searchterm = this.form.controls.search.value?.trim().toLowerCase();

    if (searchterm != undefined) this.searched.emit({ searchterm: searchterm });
  }
}
