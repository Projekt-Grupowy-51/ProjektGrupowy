import { useNavigate } from 'react-router-dom';

export const useNavigation = () => {
  const navigate = useNavigate();

  const goTo = (path, options = {}) => {
    if (path) navigate(path, options);
  };

  const goBack = () => navigate(-1);

  return { goTo, goBack };
};

export const useFormNavigation = (successPath, cancelPath) => {
  const { goTo } = useNavigation();

  const handleSuccess = (path) => goTo(path || successPath);
  const handleCancel = (path) => goTo(path || cancelPath);

  return { handleSuccess, handleCancel };
};
