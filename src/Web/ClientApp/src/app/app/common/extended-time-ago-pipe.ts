import { Pipe } from '@angular/core';
import { TimeAgoPipe } from 'time-ago-pipe';

@Pipe({
	name: 'timeAgo',
	pure: false,
})
export class ExtendedTimeAgoPipe extends TimeAgoPipe {}
