export class GarbageCollector {
    private garbage: any[];

    constructor(garbage: any[]) {
        this.garbage = garbage;
    }

    public collect() {
        if (!this.garbage) { return; }

        this.garbage.forEach(element => {
            if (!element) { return; }
            try {
                if (this.isBehaviorSubject(element)) {
                    element.complete();
                    return;
                }

                if (this.isDynamicDialogRef(element)) {
                    element.destroy();
                    return;
                }

                if (this.isTimer(element)) {
                    clearTimeout(element);
                    return;
                }

                if (this.isSubscribtion(element)) {
                    try {
                        element.unsubscribe();
                    } catch(err) {
                        console.log(err)
                    }
                    return;
                }

            } catch(e) {
                console.log(e);
            }
        });
    }

    private isBehaviorSubject(obj: any): boolean {
        return obj.complete !== undefined;
    }

    private isDynamicDialogRef(obj: any): boolean {
        return obj.close !== undefined;
    }

    private isSubscribtion(obj: any): boolean {
        return obj.unsubscribe !== undefined;
    }

    private isTimer(obj: any): boolean {
        return obj.hasRef !== undefined ;
    }
}
