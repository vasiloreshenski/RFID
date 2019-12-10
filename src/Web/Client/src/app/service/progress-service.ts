import { Observable, Subscriber } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable()
export class ProgressService {
    private subscriber: Subscriber<boolean>;
    public loading$ = new Observable<boolean>(sub => {
        this.subscriber = sub;
    });

    public startLoading() {
        if (this.subscriber) {
            this.subscriber.next(true);
        }
    }

    public stopLoading() {
        if (this.subscriber) {
            setTimeout(() => this.subscriber.next(false), 300);
        }
    }

    public executeWithProgress<T>(obs$: Observable<T>, func: (value: T) => void): Observable<T> {
        this.startLoading();
        obs$.subscribe(value => {
            func(value);
            this.stopLoading();
        });
        return obs$;
    }
}
