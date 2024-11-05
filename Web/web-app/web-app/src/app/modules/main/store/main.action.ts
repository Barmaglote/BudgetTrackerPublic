import { createActionGroup, emptyProps } from '@ngrx/store';
import { BriefStatistics } from '../../item/models/brief-statistics';

export const MainActions  = createActionGroup({
  source: 'Main',
  events: {
    'Clean Store': emptyProps(),
    'Get Brief Statistics': emptyProps(),
    'Get Brief Statistics Success': (briefStatistics: BriefStatistics) => ({briefStatistics}),
    'Failure': (text: string, error: any) => ({text, error}),
  },
});
