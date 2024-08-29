import {Component} from '@angular/core';
import {TrainsService} from 'src/app/services/trains.service';
import {IRoute, ISearchTrain} from "../../models/Station";

@Component({
  selector: 'app-trains',
  templateUrl: './trains.component.html',
  styleUrls: ['./trains.component.css']
})
export class TrainsComponent {

  trainList: ISearchTrain [] = [];

  constructor(private trainSrv: TrainsService) {
    this.getAllTrains();
  }

  async getAllTrains() {
    this.trainList = await this.trainSrv.getAllRoutes() || [];
  }

}
