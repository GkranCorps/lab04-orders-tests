"""Проверка 100% покрытия по исходному отчёту Coverlet, без округления."""
import argparse
import html
import hashlib
from pathlib import Path
import xml.etree.ElementTree as ET

parser=argparse.ArgumentParser()
parser.add_argument('folder')
parser.add_argument('--enforce',action='store_true')
args=parser.parse_args()
folder=Path(args.folder)
files=list(folder.rglob('coverage.cobertura.xml'))
# TRX может сохранять вторую идентичную копию вложения.
files=list({hashlib.sha256(path.read_bytes()).hexdigest():path for path in files}.values())
if len(files)!=1:
    raise SystemExit(f'Ожидался один отчёт покрытия, найдено: {len(files)}')
root=ET.parse(files[0]).getroot()
lc,lv,bc,bv=(int(root.attrib[k]) for k in ['lines-covered','lines-valid','branches-covered','branches-valid'])
if lv==0 or bv==0:
    raise SystemExit('Пустой отчёт покрытия не принимается.')
summary=f'Строки: {lc}/{lv} ({100*lc/lv:.2f}%). Ветви: {bc}/{bv} ({100*bc/bv:.2f}%).'
print(summary)
entries=[]
for klass in root.findall('.//class'):
    missing=[l.attrib['number'] for l in klass.findall('./lines/line') if int(l.attrib['hits'])==0]
    entries.append('<tr><td>'+html.escape(klass.attrib['name'])+'</td><td>'+f"{100*float(klass.attrib['line-rate']):.2f}%"+'</td><td>'+f"{100*float(klass.attrib['branch-rate']):.2f}%"+'</td><td>'+(', '.join(missing) or 'нет')+'</td></tr>')
content='''<!doctype html><html lang="ru"><meta charset="utf-8"><title>Покрытие тестами</title><style>body{font:18px Arial;max-width:1050px;margin:45px auto;padding:20px;color:#172534}h1{font-size:30px}.summary{background:#eaf2fb;padding:22px;font-size:24px}table{width:100%;border-collapse:collapse;margin-top:30px}th,td{border:1px solid #ccd4de;padding:14px;text-align:left}th{background:#eef2f7}.note{margin-top:25px;color:#526070;font-size:16px}</style><h1>Shop.Orders — отчёт покрытия</h1>'''
content+='<p class="summary">'+html.escape(summary)+'</p><table><tr><th>Класс</th><th>Строки</th><th>Ветви</th><th>Непокрытые строки</th></tr>'+''.join(entries)+'</table><p class="note">Источник: фактический coverage.cobertura.xml от coverlet.collector. Область: вся сборка Shop.Orders; исключений файлов и классов нет. Автоматические свойства включены. Тестовая сборка не является тестируемым приложением.</p></html>'
(folder/'coverage.html').write_text(content,encoding='utf-8')
(folder/'summary.txt').write_text(summary,encoding='utf-8')
if args.enforce and (lc!=lv or bc!=bv):
    raise SystemExit('Не выполнен порог 100% строк и 100% ветвей.')
