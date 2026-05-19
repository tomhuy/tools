import { Injectable, computed, signal } from '@angular/core';
import { ActivityItem, YearKey } from './models/activity-value-matrix.model';

export const YEAR_INDEX: Record<YearKey, number> = { 1: 0, 3: 1, 5: 2, 10: 3 };

const ACTIVITY_DATA: ActivityItem[] = [
  {
    id: 'note',
    name: 'Ghi chép',
    icon: '📝',
    color: '#1D9E75',
    colorLight: 'rgba(29,158,117,0.15)',
    timeInvest: 20,
    desc: 'Ghi chú, journal, capture ý tưởng',
    quad: 'tl',
    pos: { x: 30, y: 25 },
    metrics: {
      compound: [1.4, 2.8, 5.2, 18],
      clarity:  [15, 35, 60, 90],
      network:  [8, 22, 45, 80],
    },
    bars: [
      { label: 'Tốc độ học (compounding)', color: '#1D9E75' },
      { label: 'Clarity tư duy',           color: '#5DCAA5' },
      { label: 'Kết nối ý tưởng',          color: '#97C459' },
    ],
    insights: {
      1:  'Sau 1 năm: Bắt đầu thấy các pattern tư duy của bản thân. Ý tưởng cũ bắt đầu kết nối với nhau.',
      3:  'Sau 3 năm: Bạn có một knowledge base cá nhân không ai có. Mỗi ý tưởng mới kết nối với hàng trăm insight cũ — tốc độ học tăng gấp đôi.',
      5:  'Sau 5 năm: Ghi chép trở thành "second brain" thực sự. Bạn có thể viết, dạy, và build từ kho kiến thức tích lũy — người khác mất 5 năm mới có được.',
      10: 'Sau 10 năm: Tài sản tri thức không ai lấy đi được. Mỗi ngày bạn đang đứng trên vai của 10 năm bản thân trước đó.',
    },
  },
  {
    id: 'learn',
    name: 'Học kiến thức mới',
    icon: '📚',
    color: '#7F77DD',
    colorLight: 'rgba(127,119,221,0.15)',
    timeInvest: 35,
    desc: 'Đọc sách, khóa học, nghiên cứu',
    quad: 'tr',
    pos: { x: 28, y: 30 },
    metrics: {
      compound: [1.3, 2.2, 4.1, 12],
      clarity:  [20, 45, 70, 95],
      network:  [12, 30, 55, 85],
    },
    bars: [
      { label: 'Depth tư duy',     color: '#7F77DD' },
      { label: 'Mental models',    color: '#AFA9EC' },
      { label: 'Transfer học tập', color: '#534AB7' },
    ],
    insights: {
      1:  'Sau 1 năm: Mental models mới bắt đầu thay đổi cách bạn nhìn vấn đề. Bạn thấy pattern mà người khác bỏ qua.',
      3:  'Sau 3 năm: Bạn có nền tảng cross-domain hiếm — kết nối tâm lý học, design, engineering. Mỗi kiến thức mới học nhanh hơn vì có framework sẵn.',
      5:  'Sau 5 năm: Tư duy hệ thống trở thành bản năng. Bạn giải quyết vấn đề ở tầng gốc rễ trong khi người khác vẫn đang xử lý triệu chứng.',
      10: 'Sau 10 năm: Không còn ai có cùng combination kiến thức với bạn. Đây là lợi thế cạnh tranh không thể copy.',
    },
  },
  {
    id: 'apply',
    name: 'Áp dụng kiến thức',
    icon: '⚡',
    color: '#EF9F27',
    colorLight: 'rgba(239,159,39,0.15)',
    timeInvest: 40,
    desc: 'Thực hành, experiment, feedback loop',
    quad: 'tr',
    pos: { x: 62, y: 55 },
    metrics: {
      compound: [1.8, 3.5, 7.2, 25],
      clarity:  [25, 55, 80, 98],
      network:  [20, 45, 75, 92],
    },
    bars: [
      { label: 'Skill compounding',    color: '#EF9F27' },
      { label: 'Intuition chuyên môn', color: '#FAC775' },
      { label: 'Feedback loop speed',  color: '#BA7517' },
    ],
    insights: {
      1:  'Sau 1 năm: Khoảng cách giữa biết và làm được thu hẹp nhanh. Bạn fail nhanh, learn nhanh hơn người chỉ đọc.',
      3:  'Sau 3 năm: Intuition chuyên môn bắt đầu hình thành — bạn nhận ra giải pháp đúng trước khi phân tích xong. Đây là thứ không học được từ sách.',
      5:  'Sau 5 năm: Pattern recognition sâu đến mức bạn xử lý vấn đề phức tạp gần như tự động. ROI của mỗi giờ làm việc cao hơn người mới gấp 5–10 lần.',
      10: 'Sau 10 năm: Malcolm Gladwell gọi đây là 10,000 giờ — nhưng thực ra là 10 năm feedback loop chất lượng. Expertise thực sự, không thể giả.',
    },
  },
  {
    id: 'build',
    name: 'Build app',
    icon: '🛠',
    color: '#D4537E',
    colorLight: 'rgba(212,83,126,0.12)',
    timeInvest: 65,
    desc: 'Coding, ship feature, tinkering',
    quad: 'br',
    pos: { x: 55, y: 40 },
    metrics: {
      compound: [1.1, 1.4, 1.8, 2.5],
      clarity:  [5, 10, 18, 30],
      network:  [10, 20, 32, 50],
    },
    bars: [
      { label: 'Output ngắn hạn', color: '#D4537E' },
      { label: 'Compound value',  color: '#ED93B1' },
      { label: 'Wisdom tích lũy', color: '#993556' },
    ],
    insights: {
      1:  'Sau 1 năm: Output có thể nhìn thấy nhưng nếu thiếu nền tảng 1+2+3, bạn đang build trên cát. Technical debt tích lũy nhanh.',
      3:  'Sau 3 năm: Người chỉ build mà không học và ghi chép sẽ thấy mình giải đi giải lại cùng một vấn đề. Không có compound effect.',
      5:  'Sau 5 năm: Nếu không có nền tảng 1+2+3, bạn có thể trở thành senior coder nhưng không bao giờ là problem solver thực sự. Tool tốt nhưng không có wisdom.',
      10: 'Sau 10 năm: Build app đứng một mình không compound. Nhưng Build + (Ghi chép + Học + Áp dụng) = exponential. Thiếu 3 cái kia, cái này chỉ là linear.',
    },
  },
];

@Injectable({ providedIn: 'root' })
export class ActivityValueMatrixService {
  readonly activities = signal<ActivityItem[]>(ACTIVITY_DATA);
  readonly currentYear = signal<YearKey>(3);
  readonly selectedId = signal<string | null>(null);

  readonly selectedActivity = computed(() =>
    this.activities().find(a => a.id === this.selectedId()) ?? null
  );

  readonly activitiesForQuad = (quad: string) =>
    computed(() => this.activities().filter(a => a.quad === quad));

  setYear(year: YearKey): void {
    this.currentYear.set(year);
  }

  selectActivity(id: string): void {
    this.selectedId.set(id);
  }

  getBubbleSize(activity: ActivityItem): number {
    const idx = YEAR_INDEX[this.currentYear()];
    const compound = activity.metrics.compound[idx];
    const base = activity.id === 'build' ? 54 : 52;
    return Math.round(base + compound * 10);
  }
}
