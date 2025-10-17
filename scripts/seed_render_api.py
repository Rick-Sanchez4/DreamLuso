#!/usr/bin/env python3
"""
Script para popular a API do DreamLuso no Render
Cria usuários com hashes de senha corretos através da API
"""

import requests
import time

API_URL = "https://dreamluso-api.onrender.com/api"

def wait_for_api():
    """Aguarda a API ficar disponível"""
    print("🔄 Aguardando API ficar online...")
    max_retries = 30
    for i in range(max_retries):
        try:
            response = requests.get("https://dreamluso-api.onrender.com/health", timeout=10)
            if response.status_code == 200:
                print("✅ API está online!")
                return True
        except:
            pass
        
        print(f"   Tentativa {i+1}/{max_retries}...")
        time.sleep(10)
    
    return False

def register_user(user_data):
    """Registra um novo usuário"""
    try:
        response = requests.post(
            f"{API_URL}/accounts/register",
            json=user_data,
            timeout=30
        )
        if response.status_code in [200, 201]:
            print(f"   ✅ {user_data['email']} criado com sucesso")
            return True
        else:
            print(f"   ⚠️  {user_data['email']}: {response.status_code} - {response.text[:100]}")
            return False
    except Exception as e:
        print(f"   ❌ Erro ao criar {user_data['email']}: {str(e)}")
        return False

def main():
    print("🚀 Script de População da API DreamLuso no Render")
    print("=" * 60)
    
    # Aguardar API
    if not wait_for_api():
        print("❌ API não ficou disponível")
        return
    
    print("\n👥 Criando usuários...")
    
    users = [
        {
            "firstName": "Admin",
            "lastName": "DreamLuso",
            "email": "admin@dreamluso.com",
            "password": "Admin123!",
            "phone": "912345678",
            "role": 3  # Admin
        },
        {
            "firstName": "João",
            "lastName": "Silva",
            "email": "joao.silva@dreamluso.pt",
            "password": "Agent123!",
            "phone": "918765432",
            "role": 2  # RealEstateAgent
        },
        {
            "firstName": "Maria",
            "lastName": "Santos",
            "email": "maria.santos@dreamluso.pt",
            "password": "Agent123!",
            "phone": "917654321",
            "role": 2  # RealEstateAgent
        },
        {
            "firstName": "Ana",
            "lastName": "Rodrigues",
            "email": "ana.rodrigues@email.com",
            "password": "Client123!",
            "phone": "915432109",
            "role": 1  # Client
        }
    ]
    
    for user in users:
        register_user(user)
        time.sleep(2)  # Aguardar entre requests
    
    print("\n✅ População concluída!")
    print("\n🔑 Credenciais:")
    print("   Admin: admin@dreamluso.com / Admin123!")
    print("   Agente: joao.silva@dreamluso.pt / Agent123!")
    print("   Cliente: ana.rodrigues@email.com / Client123!")

if __name__ == "__main__":
    main()

