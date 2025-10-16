#!/usr/bin/env python3
"""
Script completo para popular o sistema DreamLuso com dados realistas
Inclui: Usuários, Clientes, Agentes, Propriedades, Propostas, Visitas e Contratos
"""

import requests
import json
import time
from typing import Dict, Any, Optional, List
from datetime import datetime, timedelta

API_URL = "http://localhost:5149/api"

class SystemPopulator:
    def __init__(self):
        self.admin_token = None
        self.users = {}
        self.clients = {}
        self.agents = {}
        self.properties = {}

    def get_headers(self):
        headers = {"Content-Type": "application/json"}
        if self.admin_token:
            headers["Authorization"] = f"Bearer {self.admin_token}"
        return headers

    def login_admin(self):
        print("🔐 Fazendo login como Admin...")
        response = requests.post(f"{API_URL}/accounts/login", json={
            "email": "admin@gmail.com",
            "password": "Admin123!"
        })
        if response.status_code == 200:
            data = response.json()
            self.admin_token = data.get("accessToken") or data.get("token")
            print(f"✅ Login bem-sucedido! Token: {self.admin_token[:30]}...")
            return True
        else:
            print(f"❌ Erro no login: {response.status_code} - {response.text}")
            return False

    def create_agent_with_password(self, first_name: str, last_name: str, email: str, password: str, 
                                   license: str, specialization: str, commission: float) -> Optional[str]:
        """Cria um novo agente com senha"""
        print(f"\n👔 Criando agente: {first_name} {last_name}")
        
        # 1. Registrar usuário
        reg_response = requests.post(f"{API_URL}/accounts/register", json={
            "firstName": first_name,
            "lastName": last_name,
            "email": email,
            "password": password,
            "confirmPassword": password,
            "phone": "+351 91 000 0000",
            "role": "RealEstateAgent"
        })
        
        if reg_response.status_code not in [200, 201]:
            print(f"   ⚠️  Erro ao registrar: {reg_response.status_code}")
            return None
        
        reg_data = reg_response.json()
        user_id = reg_data.get("userId") or reg_data.get("user", {}).get("id")
        
        if not user_id:
            print(f"   ⚠️  UserID não retornado no registro")
            return None
        
        print(f"   ✅ Usuário criado: {user_id}")
        time.sleep(1)
        
        # 2. Criar perfil de agente
        agent_response = requests.post(f"{API_URL}/agents",
            headers=self.get_headers(),
            json={
                "userId": user_id,
                "licenseNumber": license,
                "licenseExpiry": "2027-12-31T00:00:00",
                "officeEmail": email,
                "officePhone": "+351 21 123 4567",
                "commissionRate": commission,
                "specialization": specialization,
                "certifications": ["AMI", "Certificado Profissional"],
                "languagesSpoken": [0, 1, 2]  # Portuguese, English, Spanish
            })
        
        if agent_response.status_code in [200, 201]:
            agent_data = agent_response.json()
            agent_id = agent_data.get("agentId") or agent_data.get("id")
            print(f"   ✅ Agente criado: {agent_id}")
            self.agents[email] = {"id": agent_id, "user_id": user_id, "password": password}
            return agent_id
        else:
            print(f"   ⚠️  Erro ao criar agente: {agent_response.status_code} - {agent_response.text[:100]}")
            return None

    def approve_agent(self, agent_id: str):
        """Aprova um agente"""
        response = requests.put(f"{API_URL}/agents/{agent_id}/approve",
            headers=self.get_headers(),
            json={"isApproved": True})
        
        if response.status_code in [200, 204]:
            print(f"   ✅ Agente aprovado!")
            return True
        else:
            print(f"   ⚠️  Erro ao aprovar agente: {response.status_code}")
            return False

    def create_property(self, agent_id: str, data: Dict) -> Optional[str]:
        """Cria uma propriedade"""
        response = requests.post(f"{API_URL}/properties",
            headers=self.get_headers(),
            json=data)
        
        if response.status_code in [200, 201]:
            prop_data = response.json()
            prop_id = prop_data.get("id")
            print(f"   ✅ Propriedade criada: {data['title']}")
            return prop_id
        else:
            print(f"   ⚠️  Erro ao criar propriedade: {response.status_code}")
            return None

    def create_proposal(self, client_id: str, property_id: str, value: float) -> Optional[str]:
        """Cria uma proposta"""
        response = requests.post(f"{API_URL}/proposals",
            headers=self.get_headers(),
            json={
                "clientId": client_id,
                "propertyId": property_id,
                "proposedValue": value,
                "type": 0,  # Purchase
                "paymentMethod": 0,  # Cash
                "additionalNotes": "Proposta criada via script de população"
            })
        
        if response.status_code in [200, 201]:
            prop_data = response.json()
            print(f"   ✅ Proposta criada: €{value:,.0f}")
            return str(prop_data) if isinstance(prop_data, str) else prop_data.get("id")
        else:
            print(f"   ⚠️  Erro ao criar proposta: {response.status_code}")
            return None

    def schedule_visit(self, client_id: str, property_id: str, agent_id: str, days_ahead: int = 3) -> Optional[str]:
        """Agenda uma visita"""
        visit_date = (datetime.now() + timedelta(days=days_ahead)).strftime("%Y-%m-%d")
        
        response = requests.post(f"{API_URL}/visits/schedule",
            headers=self.get_headers(),
            json={
                "propertyId": property_id,
                "clientId": client_id,
                "realEstateAgentId": agent_id,
                "visitDate": f"{visit_date}T00:00:00",
                "timeSlot": "14:00-15:00",
                "notes": "Visita agendada via script"
            })
        
        if response.status_code in [200, 201]:
            print(f"   ✅ Visita agendada para {visit_date}")
            return "success"
        else:
            print(f"   ⚠️  Erro ao agendar visita: {response.status_code}")
            return None

    def run_population(self):
        if not self.login_admin():
            return

        print("\n" + "="*70)
        print("🚀 INICIANDO POPULAÇÃO COMPLETA DO SISTEMA")
        print("="*70)

        # 1. Criar novo agente com senha
        print("\n📋 PASSO 1: Criando Novo Agente Imobiliário")
        print("-" * 70)
        agent_id = self.create_agent_with_password(
            "Ricardo", 
            "Fernandes",
            "ricardo.fernandes@dreamluso.pt",
            "Agente123!",
            "AMI-45678",
            "Luxury & Premium",
            0.045
        )
        
        if agent_id:
            # 2. Aprovar agente
            print("\n📋 PASSO 2: Aprovando Agente")
            print("-" * 70)
            self.approve_agent(agent_id)
            time.sleep(2)

            # 3. Criar propriedades para este agente
            print("\n📋 PASSO 3: Criando Propriedades do Agente")
            print("-" * 70)
            
            properties_data = [
                {
                    "title": "Penthouse Luxo na Avenida da Liberdade",
                    "description": "Penthouse exclusiva com 300m², terraço panorâmico, vista para o Tejo, acabamentos de luxo.",
                    "realEstateAgentId": agent_id,
                    "price": 1200000.00,
                    "size": 300.0,
                    "bedrooms": 4,
                    "bathrooms": 3,
                    "type": 0,  # Apartment
                    "status": 0,  # Available
                    "transactionType": 0,  # Sale
                    "street": "Avenida da Liberdade",
                    "number": "180",
                    "parish": "Santo António",
                    "municipality": "Lisboa",
                    "district": "Lisboa",
                    "postalCode": "1250-146",
                    "grossArea": 350.0,
                    "floor": 10,
                    "parkingSpaces": 3,
                    "hasElevator": True,
                    "hasPool": True,
                    "isFurnished": True,
                    "energyRating": "A+",
                    "yearBuilt": 2020
                },
                {
                    "title": "Moradia de Luxo em Cascais com Vista Mar",
                    "description": "Moradia isolada com 5 quartos, piscina infinity, jardim privativo, garagem para 4 carros.",
                    "realEstateAgentId": agent_id,
                    "price": 2500000.00,
                    "size": 450.0,
                    "bedrooms": 5,
                    "bathrooms": 4,
                    "type": 1,  # House
                    "status": 0,
                    "transactionType": 0,
                    "street": "Rua das Flores",
                    "number": "25",
                    "parish": "Cascais",
                    "municipality": "Cascais",
                    "district": "Lisboa",
                    "postalCode": "2750-283",
                    "grossArea": 550.0,
                    "landArea": 800.0,
                    "parkingSpaces": 4,
                    "hasPool": True,
                    "hasGarden": True,
                    "isFurnished": False,
                    "energyRating": "A",
                    "yearBuilt": 2019
                },
                {
                    "title": "Apartamento T3 Moderno no Parque das Nações",
                    "description": "Apartamento novo com 3 quartos, varanda, condomínio com ginásio e jardim.",
                    "realEstateAgentId": agent_id,
                    "price": 485000.00,
                    "size": 135.0,
                    "bedrooms": 3,
                    "bathrooms": 2,
                    "type": 0,
                    "status": 0,
                    "transactionType": 0,
                    "street": "Alameda dos Oceanos",
                    "number": "45",
                    "parish": "Parque das Nações",
                    "municipality": "Lisboa",
                    "district": "Lisboa",
                    "postalCode": "1990-207",
                    "grossArea": 145.0,
                    "floor": 5,
                    "parkingSpaces": 2,
                    "hasElevator": True,
                    "isFurnished": False,
                    "energyRating": "A",
                    "yearBuilt": 2021
                }
            ]

            created_props = []
            for prop in properties_data:
                prop_id = self.create_property(agent_id, prop)
                if prop_id:
                    created_props.append(prop_id)
                    self.properties[prop_id] = prop
                time.sleep(1)

            # 4. Obter clientes existentes
            print("\n📋 PASSO 4: Buscando Clientes Existentes")
            print("-" * 70)
            clients_response = requests.get(f"{API_URL}/clients?pageSize=10", headers=self.get_headers())
            if clients_response.status_code == 200:
                clients_data = clients_response.json()
                existing_clients = clients_data.get("clients", [])
                print(f"   Encontrados {len(existing_clients)} clientes")
                
                # 5. Criar propostas dos clientes para as propriedades
                if len(existing_clients) > 0 and len(created_props) > 0:
                    print("\n📋 PASSO 5: Criando Propostas")
                    print("-" * 70)
                    
                    # Cliente 1 faz proposta na propriedade 1
                    if len(existing_clients) >= 1 and len(created_props) >= 1:
                        self.create_proposal(
                            existing_clients[0]["id"],
                            created_props[0],
                            1100000.00  # 100k abaixo do preço
                        )
                    
                    # Cliente 2 faz proposta na propriedade 2
                    if len(existing_clients) >= 2 and len(created_props) >= 2:
                        self.create_proposal(
                            existing_clients[1]["id"],
                            created_props[1],
                            2400000.00  # 100k abaixo
                        )
                    
                    # Cliente 3 faz proposta na propriedade 3
                    if len(existing_clients) >= 3 and len(created_props) >= 3:
                        self.create_proposal(
                            existing_clients[2]["id"],
                            created_props[2],
                            480000.00  # 5k abaixo
                        )
                    
                    # Cliente 1 também faz proposta na propriedade 3
                    if len(existing_clients) >= 1 and len(created_props) >= 3:
                        self.create_proposal(
                            existing_clients[0]["id"],
                            created_props[2],
                            475000.00  # 10k abaixo
                        )

                    time.sleep(2)

                    # 6. Agendar visitas
                    print("\n📋 PASSO 6: Agendando Visitas")
                    print("-" * 70)
                    
                    if len(existing_clients) >= 1 and len(created_props) >= 1:
                        self.schedule_visit(existing_clients[0]["id"], created_props[0], agent_id, 3)
                    
                    if len(existing_clients) >= 2 and len(created_props) >= 2:
                        self.schedule_visit(existing_clients[1]["id"], created_props[1], agent_id, 5)
                    
                    if len(existing_clients) >= 3 and len(created_props) >= 3:
                        self.schedule_visit(existing_clients[2]["id"], created_props[2], agent_id, 7)

        # Summary
        print("\n" + "="*70)
        print("✅ POPULAÇÃO DO SISTEMA CONCLUÍDA!")
        print("="*70)
        print(f"\n📊 RESUMO:")
        print(f"   • 1 Novo Agente criado e aprovado")
        print(f"   • {len(created_props)} Propriedades criadas")
        print(f"   • Propostas e Visitas criadas")
        print(f"\n🔑 CREDENCIAIS DO NOVO AGENTE:")
        print(f"   Email: ricardo.fernandes@dreamluso.pt")
        print(f"   Senha: Agente123!")
        print(f"\n🌐 ACESSE:")
        print(f"   Admin: http://localhost:4200/admin/dashboard")
        print(f"   Agente: http://localhost:4200/agent/dashboard")
        print(f"   Cliente: http://localhost:4200/client/dashboard")

if __name__ == "__main__":
    populator = SystemPopulator()
    populator.run_population()

