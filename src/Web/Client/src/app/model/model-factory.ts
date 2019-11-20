import { DateTime } from './date-time';
import { StatUserOverview } from './stat-user-overview';
import { RegisterTagRequestModel } from './register-tag-request-model';
import { UnknownTag } from './unknown-tag';
import { UpdateTagRequestModel } from './update-tag-request-model';
import { TagUser } from 'src/app/model/tag-user';
import { Tag } from './tag';
import { RegisterAccessPointRequestModel } from './register-access-point-request-model';
import { UnknownAccessPoint } from './unknown-access-point';
import { DirectionType } from './direction-type';
import { UpdateAccessPointRequestModel } from './update-access-point-request-model';
import { AccessLevelType } from './access-level-type';
import { AccessPoint } from './access-point';
import { StatUser } from './stat-user';
export class ModelFactory {

    public static accessPointFromJson(json: any): AccessPoint {
        const obj = new AccessPoint();
        obj.accessLevel = json.accessLevel;
        obj.createDate = new Date(json.createDate);
        obj.description = json.description;
        obj.direction = json.direction;
        obj.id = json.id;
        obj.isActive = json.isActive;
        if (json.modificationDate) {
            obj.modificationDate = new Date(json.modificationDate);
        }
        obj.serialNumber = json.serialNumber;
        obj.canEdit = false;

        return obj;
    }

    public static unknownAccessPointFromJson(json: any): UnknownAccessPoint {
        const obj = new UnknownAccessPoint();
        obj.id = json.id;
        obj.serialNumber = json.serialNumber;
        obj.accessDate = new Date(json.accessDate);

        return obj;
    }

    public static updateAccessPointRequestModel(
        id: number,
        description: string,
        accessLevelText: string,
        directionText: string,
        isActive: boolean): UpdateAccessPointRequestModel {
        const obj = new UpdateAccessPointRequestModel();
        obj.id = id;
        obj.accessLevel = AccessLevelType[accessLevelText];
        obj.description = description;
        obj.direction = DirectionType[directionText];
        obj.isActive = isActive;
        return obj;
    }

    public static registerAccessPointRequestModel(
        serialNumber: string,
        description: string,
        accessLevelText: string,
        directionText: string,
        isActive: boolean
    ) {
        const obj = new RegisterAccessPointRequestModel();
        obj.serialNumber = serialNumber;
        obj.description = description;
        obj.accessLevel = AccessLevelType[accessLevelText];
        obj.direction = DirectionType[directionText];
        obj.isActive = isActive;
        return obj;
    }

    public static tagFromJson(json: any): Tag {
        const obj = new Tag();
        obj.accessLevel = json.accessLevel;
        obj.createDate = new Date(json.createDate);
        obj.id = json.id;
        obj.isActive = json.isActive;
        if (json.modificationDate) {
            obj.modificationDate = new Date(json.modificationDate);
        }
        obj.number = json.number;
        obj.userName = json.userName;
        obj.editMode = false;
        return obj;
    }

    public static tagUserFromJson(json: any): TagUser {
        const obj = new TagUser();
        obj.id = json.id;
        obj.userName = json.userName;
        return obj;
    }

    public static updateTagRequestModel(id: number, userName: string, accessLevelText: string) {
        const obj = new UpdateTagRequestModel();
        obj.id = id;
        obj.userName = userName;
        obj.accessLevel = AccessLevelType[accessLevelText];
        return obj;
    }

    public static unknownTagFromJson(json: any): UnknownTag {
        const obj = new UnknownTag();
        obj.id = json.id;
        obj.accessDate = json.accessDate;
        obj.number = json.number;
        return obj;
    }

    public static registerTagRequestModel(number: string, userName: string, accessLevelText: string) {
        const obj = new RegisterTagRequestModel();
        obj.number = number;
        obj.userName = userName;
        obj.accessLevel = AccessLevelType[accessLevelText];
        return obj;
    }

    public static statUserOverviewFromJson(json: any): StatUserOverview {
        const obj = new StatUserOverview();
        obj.avgEntranceTime = json.avgEntranceTime;
        obj.avgExitTime = json.avgExitTime;
        obj.avgWorkHourNorm = json.avgWorkHourNorm;
        return obj;
    }

    public static statUserFromJson(json: any): StatUser {
        const obj = new StatUser();
        obj.id = json.id;
        obj.userName = json.userName;
        obj.avgEntranceTime = json.avgEntranceTime;
        obj.avgExitTime = json.avgExitTime;
        obj.avgWorkHourNorm = json.avgWorkHourNorm;
        return obj;
    }

    public static dateTimeFromJson(json: any): DateTime {
        const obj = new DateTime();
        obj.day = new Date(json.day);
        obj.time = json.time;
        return obj;
    }

    public static dateTimeAsWorkHourNormChartJson(dateTime: DateTime): any {
        return {
            name: dateTime.day.toLocaleDateString(),
            value: dateTime.getTimeInMinutes()
        };
    }

    public static dateTimeAsEntranceExitChartJson(dateTime: DateTime): any {
        return {
            name: dateTime.day.toLocaleDateString(),
            value: dateTime.getTimeInMinutes()
        };
    }
}
