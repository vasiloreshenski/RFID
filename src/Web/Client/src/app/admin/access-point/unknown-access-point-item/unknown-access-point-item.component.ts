import { AccessPoint } from '../../../model/access-point';
import { UnknownAccessPoint } from '../../../model/unknown-access-point';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-unknown-access-point-item',
  templateUrl: './unknown-access-point-item.component.html',
  styleUrls: ['./unknown-access-point-item.component.css']
})
export class UnknownAccessPointItemComponent implements OnInit {
  @Output()
  public accessPointUpdate = new EventEmitter();

  @Input()
  public unknownAccessPoint: UnknownAccessPoint;
  public accessPoint: AccessPoint;

  constructor() {
    this.accessPoint = AccessPoint.default();
  }

  public activate() {
    const ap = AccessPoint.default();
    ap.serialNumber = this.unknownAccessPoint.serialNumber;
    ap.canEdit = true;
    this.accessPoint = ap;
    return false;
  }

  public update(): void {
    this.accessPointUpdate.emit();
  }

  ngOnInit() {
  }
}
