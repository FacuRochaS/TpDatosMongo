import re
import json
from datetime import datetime

def parse_whatsapp_chat(filepath):
    pattern = r'^(\d{1,2}/\d{1,2}/\d{4}), (\d{1,2}:\d{2}) - (.*?): (.+)'
    inserts = []

    with open(filepath, 'r', encoding='utf-8') as file:
        for line in file:
            match = re.match(pattern, line)
            if match:
                fecha_str, hora, autor, mensaje = match.groups()
                
                fecha = datetime.strptime(fecha_str, "%d/%m/%Y").strftime("%Y-%m-%d")
                doc = {
                    "fecha": fecha,
                    "hora": hora,
                    "autor": autor.replace('"', "'"),
                    "mensaje": mensaje.replace('"', "'")
                }
                inserts.append(doc)
    return inserts

def guardar_como_json_array(inserts, output_file):
    with open(output_file, "w", encoding="utf-8") as f:
        json.dump(inserts, f, indent=2, ensure_ascii=False)
    print(f"Archivo JSON generado: {output_file} ({len(inserts)} documentos)")

if __name__ == "__main__":
    archivo = "chat.txt"  # archivo exportado desde WhatsApp
    salida = "mensajes_compass.json"
    mensajes = parse_whatsapp_chat(archivo)
    guardar_como_json_array(mensajes, salida)
