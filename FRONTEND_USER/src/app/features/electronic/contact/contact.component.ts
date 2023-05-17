import { Component } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent {
  ngOnInit(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
  // FormData: FormGroup;
  
}
