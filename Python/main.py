#!/usr/local/bin/python3
# -*- coding: utf-8 -*-
import numpy as np

from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel


def main():
    """
    file_name: is the name of the environment binary (located in the root directory of the python project)
    worker_id: indicates which port to use for communication with the environment. For use in parallel training regimes such as A3C.
    seed: indicates the seed to use when generating random numbers during the training process.
          In environments which are deterministic, setting the seed enables reproducible experimentation by ensuring that the environment and trainers utilize the same random seed.
    side_channels: provides a way to exchange data with the Unity simulation that is not related to the reinforcement learning loop.
                   For example: configurations or properties.
                   More on them in the "Modifying the environment from Python"(https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Python-API.md#modifying-the-environment-from-python) section.
    ---
    env.reset()
    env.step()
    env.close()
    """
    channel = EngineConfigurationChannel()

    filename = "Mummy"
    env = UnityEnvironment(file_name=filename, seed=1, side_channels=[channel])

    channel.set_configuration_parameters(time_scale=2.0)

    env.reset()
    behavior_names = env.behavior_specs.keys()
    for name in behavior_names:
        print('behavior_name:', name)   # Mummy?team=0

    decision_steps, terminal_steps = env.get_steps(behavior_name="Mummy?team=0")
    """
    print('DecisionSteps')
    print('- observation:', decision_steps.obs)
    print('- reward:', decision_steps.reward)
    print('- agent_id:', decision_steps.agent_id)
    print('- action_mask:', decision_steps.action_mask)

    print('TerminalSteps')
    print('- observation:', terminal_steps.obs)
    print('- reward:', terminal_steps.reward)
    print('- agent_id:', terminal_steps.agent_id)
    print('- interrupted:', terminal_steps.interrupted)
    """

    while True:
        for i in decision_steps.agent_id:
            if i in terminal_steps.agent_id:
                continue
            env.set_action_for_agent(behavior_name="Mummy?team=0", agent_id=i, action=np.random.uniform(-1.0, 1.0, size=(2,)))
        env.step()

        decision_steps, terminal_steps = env.get_steps(behavior_name="Mummy?team=0")


if __name__ == "__main__":
    main()
