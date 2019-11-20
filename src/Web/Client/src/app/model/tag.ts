import { AccessLevelType } from 'src/app/model/access-level-type';
export class Tag {
    public id: number;
    public isActive: boolean;
    public accessLevel: number;
    public createDate: Date;
    public modificationDate: Date;
    public number: string;
    public userName: string;
    public editMode: boolean;

    public static default(): Tag {
        const obj = new Tag();
        obj.isActive = true;
        obj.accessLevel = 1;
        obj.createDate = new Date(Date.now());
        obj.editMode = false;
        return obj;
    }

    public getCreateDateDisplayText(): String {
        return this.createDate.toLocaleString();
    }

    public getModificationDateDisplayText(): String {
        return this.modificationDate ? this.modificationDate.toLocaleString() : '--/--/--';
    }

    public getAccessLevelDisplayText(): String {
        return AccessLevelType[this.accessLevel];
    }

    public isRegistered(): boolean {
        return this.id > 0;
    }
}
