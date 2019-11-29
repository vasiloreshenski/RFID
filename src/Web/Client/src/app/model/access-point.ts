import { DirectionType } from './direction-type';
import { AccessLevelType } from './access-level-type';

export class AccessPoint {
    public id: number;
    public serialNumber: string;
    public description: string;
    public accessLevel: number;
    public direction: number;
    public createDate: Date;
    public modificationDate: Date;
    public isActive: boolean;
    public isDeleted: boolean;
    public canEdit: boolean;

    static default(): AccessPoint {
        const obj = new AccessPoint();
        obj.accessLevel = 1;
        obj.direction = 1;
        obj.createDate = new Date(Date.now());
        obj.isActive = true;
        obj.isDeleted = false;
        obj.canEdit = false;

        return obj;
    }

    public getAccessLevelText() { return AccessLevelType[this.accessLevel]; }
    public getDirectionText() { return DirectionType[this.direction]; }
    public enableEdit(): void { this.canEdit = true; }
    public isRegistered(): boolean {
        return this.id > 0;
    }
    public getCreateDateDisplayText() {
        return this.createDate.toLocaleString();
    }
    public getModificationDateDisplayText() {
        return this.modificationDate ? this.modificationDate.toLocaleString() : '--/--/--';
    }
    public copy(): AccessPoint {
        const copy = new AccessPoint();
        copy.accessLevel = this.accessLevel;
        copy.canEdit = this.canEdit;
        copy.createDate = this.createDate;
        copy.description = this.description;
        copy.direction = this.direction;
        copy.id = this.id;
        copy.isActive = this.isActive;
        copy.isDeleted = this.isDeleted;
        copy.modificationDate = this.modificationDate;
        copy.serialNumber = this.serialNumber;
        return copy;
    }
}
