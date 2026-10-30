// Só roda na primeira inicialização do container (volume vazio).
db = db.getSiblingDB('kuravet');

db.createCollection('relatos_comportamento');
db.relatos_comportamento.createIndex({ petId: 1, dataRelato: -1 }, { name: 'ix_pet_data' });
db.relatos_comportamento.createIndex({ categoria: 1 }, { name: 'ix_categoria' });
db.relatos_comportamento.createIndex({ intensidade: 1 }, { name: 'ix_intensidade' });
