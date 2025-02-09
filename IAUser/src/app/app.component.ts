import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ApiService } from './services/api-service.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {

  constructor(private apiService: ApiService) {
  }
  apiResponse: string = "";
  title = 'IAUser';

  sendRequest(value: string) {
    this.apiResponse = "";
    this.apiService.getData(value).subscribe(data => {
      console.log(data);
      this.apiResponse = data.text;
    });
  }
}
