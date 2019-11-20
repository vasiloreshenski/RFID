export class DateTime {
    public time: string;
    public day: Date;
    public getTimeInMinutes(): number {
        const timeParts = this.time.split(':');
        const date = new Date();
        let minutes = Number(timeParts[0]) * 60; // hour
        minutes = minutes + (Math.sign(minutes) * Number(timeParts[1]));
        return minutes;
    }
}
