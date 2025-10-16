#!/usr/bin/env python3
"""
DreamLuso - Script Completo de Seed de Dados
Popula o banco de dados com usuários, perfis, propriedades, propostas e visitas
"""

import requests
import json
import sys
from datetime import datetime, timedelta
from typing import Optional, Dict, List

API_URL = "http://localhost:5149/api"

class DreamLusoSeeder:
    def __init__(self):
        self.admin_token: Optional[str] = None
        self.user_ids: Dict[str, str] = {}
        self.client_ids: Dict[str, str] = {}
        self.agent_ids: Dict[str, str] = {}
        self.property_ids: List[str] = []
        
    def login_admin(self) -> bool:
        """Faz login como admin e obtém token"""
        print("🔐 Fazendo login como Admin...")
        
        response = requests.post(f"{API_URL}/accounts/login", json={
            "email": "admin@gmail.com",
            "password": "Admin123!"
        })
        
        if response.status_code == 200:
            data = response.json()
            self.admin_token = data.get("accessToken")
            print(f"✅ Login bem-sucedido! Token obtido.")
            return True
        else:
            print(f"❌ Erro no login: {response.status_code}")
            print(response.text)
            return False
    
    def get_headers(self) -> Dict[str, str]:
        """Retorna headers com autenticação"""
        return {
            "Content-Type": "application/json",
            "Authorization": f"Bearer {self.admin_token}"
        }
    
    def get_all_users(self) -> List[Dict]:
        """Busca todos os usuários"""
        print("\n📋 Buscando usuários existentes...")
        
        response = requests.get(f"{API_URL}/users", headers=self.get_headers())
        
        if response.status_code == 200:
            users = response.json()
            print(f"   Encontrados {len(users)} usuários")
            return users
        else:
            print(f"   ⚠️  Erro ao buscar usuários: {response.status_code}")
            return []
    
    def create_client_profile(self, user_id: str, email: str, nif: str, 
                             min_budget: float, max_budget: float) -> Optional[str]:
        """Cria perfil de cliente"""
        
        response = requests.post(f"{API_URL}/clients", 
            headers=self.get_headers(),
            json={
                "userId": user_id,
                "nif": nif,
                "citizenCard": f"{nif}CC",
                "type": 0,  # Buyer = 0, Tenant = 1, Both = 2
                "minBudget": min_budget,
                "maxBudget": max_budget,
                "preferredContactMethod": "Email"
            })
        
        if response.status_code in [200, 201]:
            data = response.json()
            client_id = data.get("clientId") or data.get("id")
            print(f"   ✅ Cliente criado para {email}")
            return client_id
        else:
            try:
                error_data = response.json()
                error_msg = error_data.get("description", str(error_data))
            except:
                error_msg = response.text[:100]
            print(f"   ⚠️  {email}: [{response.status_code}] {error_msg}")
            return None
    
    def create_agent_profile(self, user_id: str, email: str, license: str, 
                            specialization: str, commission: float) -> Optional[str]:
        """Cria perfil de agente imobiliário"""
        
        response = requests.post(f"{API_URL}/agents",
            headers=self.get_headers(),
            json={
                "userId": user_id,
                "licenseNumber": license,
                "licenseExpiry": "2026-12-31T00:00:00",
                "officeEmail": email,
                "officePhone": "+351 21 000 0000",
                "commissionRate": commission,
                "specialization": specialization,
                "certifications": ["AMI", "Certificado Profissional"],
                "languagesSpoken": [0, 1]  # Portuguese=0, English=1
            })
        
        if response.status_code in [200, 201]:
            data = response.json()
            agent_id = data.get("agentId") or data.get("id")
            print(f"   ✅ Agente criado para {email} - Licença: {license}")
            return agent_id
        else:
            try:
                error_data = response.json()
                error_msg = error_data.get("description", str(error_data))
            except:
                error_msg = response.text[:100]
            print(f"   ⚠️  {email}: [{response.status_code}] {error_msg}")
            return None
    
    def create_property(self, agent_id: str, title: str, price: float, 
                       property_type: str, transaction_type: str, 
                       bedrooms: int, size: float, address: Dict) -> Optional[str]:
        """Cria uma propriedade"""
        
        response = requests.post(f"{API_URL}/properties",
            headers=self.get_headers(),
            json={
                "title": title,
                "description": f"Excelente {property_type.lower()} localizado em {address['municipality']}. Imóvel em ótimo estado de conservação.",
                "type": property_type,
                "transactionType": transaction_type,
                "price": price,
                "size": size,
                "bedrooms": bedrooms,
                "bathrooms": max(1, bedrooms - 1),
                "address": address,
                "realEstateAgentId": agent_id,
                "isActive": True,
                "isFeatured": price > 400000,
                "hasGarage": bedrooms >= 2,
                "hasElevator": bedrooms >= 3,
                "energyRating": "B",
                "amenities": ["Aquecimento Central", "Ar Condicionado", "Cozinha Equipada"]
            })
        
        if response.status_code in [200, 201]:
            data = response.json()
            property_id = data.get("propertyId") or data.get("id")
            print(f"   ✅ Propriedade criada: {title} - €{price:,.0f}")
            return property_id
        else:
            print(f"   ⚠️  Erro ao criar {title}: {response.status_code}")
            return None
    
    def run_seed(self):
        """Executa o seed completo"""
        
        # 1. Login
        if not self.login_admin():
            return False
        
        # 2. Buscar usuários
        users = self.get_all_users()
        
        if not users:
            print("❌ Nenhum usuário encontrado!")
            return False
        
        # 3. Criar perfis de Clientes
        print("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━")
        print("👥 Criando perfis de Clientes...")
        print("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━")
        
        clients_data = [
            ("ana.rodrigues@email.com", "123456789", 200000, 400000),
            ("carlos.ferreira@email.com", "234567890", 300000, 600000),
            ("sofia.almeida@email.com", "345678901", 150000, 300000),
            ("miguel.oliveira@email.com", "456789012", 100000, 250000),
            ("beatriz.lopes@email.com", "567890123", 250000, 500000),
        ]
        
        for user in users:
            if user["role"] == "Client":
                email = user["email"]
                for client_email, nif, min_b, max_b in clients_data:
                    if client_email == email:
                        client_id = self.create_client_profile(
                            user["id"], email, nif, min_b, max_b
                        )
                        if client_id:
                            self.client_ids[email] = client_id
                        break
        
        # 4. Criar perfis de Agentes
        print("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━")
        print("👔 Criando perfis de Agentes...")
        print("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━")
        
        agents_data = [
            ("joao.silva@dreamluso.pt", "AMI-12345", "Residencial", 3.5),
            ("maria.santos@dreamluso.pt", "AMI-23456", "Luxury", 4.0),
            ("pedro.costa@dreamluso.pt", "AMI-34567", "Comercial", 3.0),
        ]
        
        for user in users:
            if user["role"] == "RealEstateAgent":
                email = user["email"]
                for agent_email, license, spec, commission in agents_data:
                    if agent_email == email:
                        agent_id = self.create_agent_profile(
                            user["id"], email, license, spec, commission
                        )
                        if agent_id:
                            self.agent_ids[email] = agent_id
                            self.user_ids[email] = user["id"]
                        break
        
        # 5. Criar Propriedades
        print("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━")
        print("🏠 Criando Propriedades...")
        print("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━")
        
        if self.agent_ids:
            # Usar o primeiro agente disponível
            first_agent_id = list(self.agent_ids.values())[0]
            
            properties_data = [
                {
                    "title": "Apartamento T3 Moderno em Lisboa",
                    "price": 385000,
                    "type": "Apartment",
                    "transaction": "Sale",
                    "bedrooms": 3,
                    "size": 120.5,
                    "address": {
                        "street": "Avenida da República",
                        "number": "125",
                        "postalCode": "1050-190",
                        "parish": "Alvalade",
                        "municipality": "Lisboa",
                        "district": "Lisboa"
                    }
                },
                {
                    "title": "Moradia V4 com Jardim no Porto",
                    "price": 650000,
                    "type": "House",
                    "transaction": "Sale",
                    "bedrooms": 4,
                    "size": 280,
                    "address": {
                        "street": "Rua do Ouro",
                        "number": "45",
                        "postalCode": "4150-553",
                        "parish": "Foz do Douro",
                        "municipality": "Porto",
                        "district": "Porto"
                    }
                },
                {
                    "title": "T2 Arrendamento em Cascais",
                    "price": 1200,
                    "type": "Apartment",
                    "transaction": "Rent",
                    "bedrooms": 2,
                    "size": 85,
                    "address": {
                        "street": "Avenida Marginal",
                        "number": "2050",
                        "postalCode": "2750-001",
                        "parish": "Cascais",
                        "municipality": "Cascais",
                        "district": "Lisboa"
                    }
                },
                {
                    "title": "Loja Comercial Centro de Braga",
                    "price": 1800,
                    "type": "Commercial",
                    "transaction": "Rent",
                    "bedrooms": 0,
                    "size": 150,
                    "address": {
                        "street": "Rua do Souto",
                        "number": "88",
                        "postalCode": "4700-329",
                        "parish": "Braga (Maximinos)",
                        "municipality": "Braga",
                        "district": "Braga"
                    }
                },
                {
                    "title": "T1 Estudante em Coimbra",
                    "price": 550,
                    "type": "Apartment",
                    "transaction": "Rent",
                    "bedrooms": 1,
                    "size": 55,
                    "address": {
                        "street": "Rua da Sofia",
                        "number": "142",
                        "postalCode": "3000-392",
                        "parish": "Coimbra (Sé Nova)",
                        "municipality": "Coimbra",
                        "district": "Coimbra"
                    }
                }
            ]
            
            # Distribuir propriedades entre agentes
            agent_list = list(self.agent_ids.values())
            
            for idx, prop_data in enumerate(properties_data):
                agent_id = agent_list[idx % len(agent_list)] if agent_list else first_agent_id
                
                prop_id = self.create_property(
                    agent_id,
                    prop_data["title"],
                    prop_data["price"],
                    prop_data["type"],
                    prop_data["transaction"],
                    prop_data["bedrooms"],
                    prop_data["size"],
                    prop_data["address"]
                )
                
                if prop_id:
                    self.property_ids.append(prop_id)
        
        # 6. Resumo
        print("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━")
        print("✅ SEED COMPLETO FINALIZADO!")
        print("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━")
        print(f"\n📊 RESUMO:")
        print(f"   • {len(self.client_ids)} Perfis de Clientes criados")
        print(f"   • {len(self.agent_ids)} Perfis de Agentes criados")
        print(f"   • {len(self.property_ids)} Propriedades criadas")
        print(f"\n🔑 LOGIN:")
        print(f"   Email: admin@gmail.com")
        print(f"   Senha: Admin123!")
        print(f"\n🌐 ACESSE: http://localhost:4200/admin/dashboard")
        print("")
        
        return True

def main():
    seeder = DreamLusoSeeder()
    
    try:
        if not seeder.login_admin():
            sys.exit(1)
        
        seeder.run_seed()
        
    except Exception as e:
        print(f"\n❌ Erro durante seed: {str(e)}")
        import traceback
        traceback.print_exc()
        sys.exit(1)

if __name__ == "__main__":
    main()

