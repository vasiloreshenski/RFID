import { TagUser } from 'src/app/model/tag-user';
import { UnknownTag } from './../../../model/unknown-tag';
import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Tag } from 'src/app/model/tag';

@Component({
  selector: 'app-unknown-tag-item',
  templateUrl: './unknown-tag-item.component.html',
  styleUrls: ['./unknown-tag-item.component.css']
})
export class UnknownTagItemComponent implements OnInit {

  @Output()
  public tagUpdateEvent = new EventEmitter();

  @Input()
  public unknownTag: UnknownTag;
  @Input()
  public users: TagUser[] = [];
  public tag: Tag = Tag.default();

  constructor() { }
  
  public activate() {
    this.tag = Tag.default();
    this.tag.number = this.unknownTag.number;
    this.tag.canEdit = true;
    return false;
  }

  public update() {
    this.tagUpdateEvent.emit();
    return false;
  }

  ngOnInit() {
  }
}
