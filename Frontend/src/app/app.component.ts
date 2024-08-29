import { Component } from '@angular/core';
import { IPassenger } from './models/Station';
import { TrainsService } from './services/trains.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  registerObj: IPassenger = new IPassenger();

  loggedUserData: any;
  constructor(private trainSrv: TrainsService) {
    const localData = localStorage.getItem('trainUser');
    if(localData != null) {
      this.loggedUserData = JSON.parse(localData);
    }
  }


  logoff() {
    localStorage.removeItem('trainUser');
    this.loggedUserData = undefined;
  }
  openRegister() {
    const model = document.getElementById('registerModel');
    if(model != null) {
      model.style.display ='block'
    }
  }

  closeRegister() {
    const model = document.getElementById('registerModel');
    if(model != null) {
      model.style.display ='none'
    }
  }

  openLogin() {
    const model = document.getElementById('loginModel');
    if(model != null) {
      model.style.display ='block'
    }
  }

  closeLogin() {
    const model = document.getElementById('loginModel');
    if(model != null) {
      model.style.display ='none'
    }
  }
  async onRegister() {

  }

  async onLogin() {

}
}
