### Passo a passo

Siga esse tutorial para criar um ambiente virtual [[link](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Using-Virtual-Environment.md)

- Resumo:
Caso não tenha o pacote que cria o ambiente virtual execute:
`sudo apt-get install python3-venv`

Passo a passo da criação do ambiente virtual:

`python3 -m venv env`
source env/bin/activate
pip3 install --upgrade pip
pip3 install --upgrade setuptools
pip3 install mlagents

#### Para executar o treinamento via Unity

mlagents-learn config/rollerball_config.yaml --run-id=<id>

#### Para executar o treinamento via executável (considerando linux)

mlagents-learn config/rollerball_config.yaml --env=GameWindow/Roller3D.x86_64 --run-id=firstTraining 

#### Para compartilhar os dados de treinamento

tensorboard dev upload --logdir=results --name "RollerBall" --description "Resultados"

Resposta [[link](https://tensorboard.dev/experiment/JqyKZ5rDREe0zDs28P7v4w/
)]
