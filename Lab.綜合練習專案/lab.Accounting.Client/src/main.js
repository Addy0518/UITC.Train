import { createApp } from 'vue';
import { createPinia } from 'pinia';
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate';
import 'sweetalert2/src/sweetalert2.scss';
import PrimeVue from 'primevue/config';
import Aura from '@primeuix/themes/aura';
import App from './App.vue';
import router from './router';
import './assets/main.css';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import TreeSelect from 'primevue/treeselect';
import InputGroup from 'primevue/inputgroup';
import InputGroupAddon from 'primevue/inputgroupaddon';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Select from 'primevue/select';
import Textarea from 'primevue/textarea';
import DatePicker from 'primevue/datepicker';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import Password from 'primevue/password';
import Chip from 'primevue/chip';
import CascadeSelect from 'primevue/cascadeselect';
import Toast from 'primevue/toast';
import ToastService from 'primevue/toastservice';
import PanelMenu from 'primevue/panelmenu';
import InValidErrorMessage from '@/common/InValidErrorMessage.vue';
import Loading from '@/common/Loading.vue';
import Rating from 'primevue/rating';
import Galleria from 'primevue/galleria';
const app = createApp(App);

const pinia = createPinia();
pinia.use(piniaPluginPersistedstate);
app.use(pinia);
app.use(router);
app.use(PrimeVue, {
  theme: {
    preset: Aura,
    options: {
      darkModeSelector: false,
      cssLayer: {
        name: 'primevue',
        order: 'theme, base, primevue',
      },
    },
  },
});

app.component('DataTable', DataTable);
app.component('Column', Column);
app.component('TreeSelect', TreeSelect);
app.component('InputGroup', InputGroup);
app.component('InputGroupAddon', InputGroupAddon);
app.component('InputText', InputText);
app.component('InputNumber', InputNumber);
app.component('Select', Select);
app.component('Textarea', Textarea);
app.component('Button', Button);
app.component('DatePicker', DatePicker);
app.component('Dialog', Dialog);
app.component('Password', Password);
app.component('Chip', Chip);
app.component('CascadeSelect', CascadeSelect);
app.component('Toast', Toast);
app.component('PanelMenu', PanelMenu);
app.component('Rating', Rating);
app.component('Galleria', Galleria);
app.component('InValidErrorMessage', InValidErrorMessage);
app.component('Loading', Loading);
app.use(ToastService);
app.mount('#app');
export { app };
