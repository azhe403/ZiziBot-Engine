import {NgModule} from '@angular/core';
import {CommonModule, NgForOf} from '@angular/common';

import {SessionRoutingModule} from './session-routing.module';
import {VerifyComponent} from './verify/verify.component';


@NgModule({
    declarations: [
        VerifyComponent
    ],
    imports: [
        CommonModule,
        SessionRoutingModule,
        NgForOf
    ]
})
export class SessionModule {
}
